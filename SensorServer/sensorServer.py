#!/usr/bin/env python
import pexpect
import sys
import time
import json
import requests

tosigned = lambda n: float(n-0x10000) if n>0x7fff else float(n)
tosignedbyte = lambda n: float(n-0x100) if n>0x7f else float(n)

def calculateTemperature(objT, ambT):
    objT = tosigned(objT)
    ambT = tosigned(ambT)
    m_tmpAmb = ambT/128.0
    Vobj2 = objT * 0.00000015625
    Tdie2 = m_tmpAmb + 273.15
    S0 = 6.4E-14            # Calibration factor
    a1 = 1.75E-3
    a2 = -1.678E-5
    b0 = -2.94E-5
    b1 = -5.7E-7
    b2 = 4.63E-9
    c2 = 13.4
    Tref = 298.15
    S = S0*(1+a1*(Tdie2 - Tref)+a2*pow((Tdie2 - Tref),2))
    Vos = b0 + b1*(Tdie2 - Tref) + b2*pow((Tdie2 - Tref),2)
    fObj = (Vobj2 - Vos) + c2*pow((Vobj2 - Vos),2)
    tObj = pow(pow(Tdie2,4) + (fObj/S),.25)
    tObj = (tObj - 273.15)
    return tObj

def calculateHumidity(rawT, rawH):
    # Calculate temperature [deg C] --
    t = -46.85 + 175.72/65536.0 * rawT
    rawH = float(int(rawH) & ~0x0003); # clear bits [1..0] (status bits)
    # Calculate relative humidity [%RH] --
    rh = -6.0 + 125.0/65536.0 * rawH # RH= -6 + 125 * SRH/2^16
    return (t, rh)

def calculateAcceleration(rawX, rawY, rawZ):
    accel = lambda v: tosignedbyte(v) / 64.0  # Range -2G, +2G
    x = accel(rawX)
    y = accel(rawY)
    z = accel(rawZ)
    mag = (x**2 + y**2 + z**2)**0.5
    return (x,y,z, mag)

def calculateBarometerTemperature(rawT, barCalib):
    #  Ta = ((c1 * Tr) / 2^24) + (c2 / 2^10)
    c1 = barCalib.c1
    c2 = barCalib.c2
    val = long((c1 * rawT) * 100)
    temp = val >> 24
    val = long(c2 * 100)
    temp += (val >> 10)
    return float(temp) / 100.0

def calculateBarometerPressure(rawP,rawT,barCalib):
    # Sensitivity = (c3 + ((c4 * Tr) / 2^17) + ((c5 * Tr^2) / 2^34))
    # Offset = (c6 * 2^14) + ((c7 * Tr) / 2^3) + ((c8 * Tr^2) / 2^19)
    # Pa = (Sensitivity * Pr + Offset) / 2^14
    Pr = rawP 
    Tr = rawT
    c3 = barCalib.c3
    c4 = barCalib.c4
    c5 = barCalib.c5
    c6 = barCalib.c6
    c7 = barCalib.c7
    c8 = barCalib.c8
    #Sensitivity
    s = long(c3)
    val = long(c4 * Tr)
    s += (val >> 17)
    val = long(c5 * Tr * Tr)
    s += (val >> 34)
    #Offset
    o = long(c6 << 14)
    val = long(c7 * Tr)
    o += (val >> 3)
    val = long(c8 * Tr * Tr)
    o += (val >> 19)
    #Pressure (Pa)
    pres = long((s * Pr) + o) >> 14
    return float(pres/100)

def calculateMagneticForce(rawX,rawY,rawZ):
    #calculate magnetic force, unit uT, range -1000, +1000
    magX = (rawX * 1.0) / (65536/2000)
    magY = (rawY * 1.0) / (65536/2000)
    magZ = (rawZ * 1.0) / (65536/2000)    
    return (magX, magY, magZ)

def calculateGyroscopeRotation(rawX,rawY,rawZ):
    #-- calculate rotation, unit deg/s, range -250, +250
    degX = (rawX * 1.0) / (65536/ 500);
    degY = (rawY * 1.0) / (65536/ 500);
    degZ = (rawZ * 1.0) / (65536/ 500);
    return (degX, degY, degZ)

class MeasurementStorage:
    temperatureSet=False
    temperature=0

    accelerationSet=False
    xAcceleration=0
    yAcceleration=0
    zAcceleration=0
    accelerationMagnitude=0

    magneticForceSet=False
    xMagneticForce=0
    yMagneticForce=0
    zMagneticForce=0

    gyroscopeSet=False
    xGyroRotation=0
    yGyroRotation=0
    zGyroRotation=0

    humiditySet=False
    humidity=0

    pressureSet=False
    pressure=0
    def __init__( self):
        return

    def setTemperature(self,temp):
        self.temperature = temp
        self.temperatureSet = True

    def setAcceleration(self,x,y,z,mag):
        self.xAcceleration=x
        self.yAcceleration=y
        self.zAcceleration=z
        self.accelerationMagnitude=mag
        self.accelerationSet=True
    
    def setGyroRotation(self,x,y,z):
        self.gyroscopeSet=True
        self.xGyroRotation=x
        self.yGyroRotation=y
        self.zGyroRotation=z

    def setHumidity(self,hum):
        self.humidity=hum
        self.humiditySet=True

    def setPressure(self,Pressure):
        self.pressure = Pressure
        self.pressureSet=True

    def setMagneticForce(self,x,y,z):
        self.xMagneticForce = x
        self.yMagneticForce = y
        self.zMagneticForce = z
        self.magneticForceSet=True
    
    def sendIfAllSet(self):
        if(self.pressureSet==True and self.humiditySet==True and self.gyroscopeSet==True and self.accelerationSet==True and self.temperatureSet == True and self.magneticForceSet==True):
            print 'Building measurement'            
            dataToSend = "<Measurements><Measurement>\
                          <deviceId>"+str(SensorID)+"</deviceId>\
                          <latitude>"+str(lat)+"</latitude>\
                          <longitude>"+str(lon)+"</longitude>\
                          <temperature>"+str(self.temperature)+"</temperature>\
                          <humidity>"+str(self.humidity)+"</humidity>\
                          <pressure>"+str(self.pressure)+"</pressure>\
                          <xacceleration>"+str(self.xAcceleration)+"</xacceleration>\
                          <yacceleration>"+str(self.yAcceleration)+"</yacceleration>\
                          <zacceleration>"+str(self.zAcceleration)+"</zacceleration>\
                          <accelerationmagnitude>"+str(self.accelerationMagnitude)+"</accelerationmagnitude>\
                          <xrotation>"+str(self.xGyroRotation)+"</xrotation>\
                          <yrotation>"+str(self.yGyroRotation)+"</yrotation>\
                          <zrotation>"+str(self.zGyroRotation)+"</zrotation>\
                          <xmagneticforce>"+str(self.xMagneticForce)+"</xmagneticforce>\
                          <ymagneticforce>"+str(self.yMagneticForce)+"</ymagneticforce>\
                          <zmagneticforce>"+str(self.zMagneticForce)+"</zmagneticforce>\
                          </Measurement></Measurements>"
            print 'Sending measurement'  
            r = requests.post(url, data=dataToSend)
            print r
            self.temperatureSet=False
            self.accelerationSet=False
            self.magneticForceSet=False
            self.gyroscopeSet=False
            self.humiditySet=False
            self.pressureSet=False
            return
        return

        
class BarometerCalibration():
    def bld_int(self, lobyte, hibyte):
        return (lobyte & 0x0FF) + ((hibyte & 0x0FF) << 8)
    
    def __init__( self, pData ):
        self.c1 = self.bld_int(pData[0],pData[1])
        self.c2 = self.bld_int(pData[2],pData[3])
        self.c3 = self.bld_int(pData[4],pData[5])
        self.c4 = self.bld_int(pData[6],pData[7])
        self.c5 = self.bld_int(pData[8],pData[9])
        self.c6 = self.bld_int(pData[10],pData[11])
        self.c7 = self.bld_int(pData[12],pData[13])
        self.c8 = self.bld_int(pData[14],pData[15])

class TISensorTag:
    def __init__( self, bluetooth_adr ):
        self.con = pexpect.spawn('gatttool -b ' + bluetooth_adr + ' --interactive')
        self.con.expect('\[LE\]>')
        print "Preparing to connect. You might need to press the side button..."
        self.con.sendline('connect')
        # test for success of connect
        self.con.expect('\[CON\].*>')
        self.cb = {}
        return
    
    def writeCommand( self, handle, value ):
        # The 0%x for value is VERY naughty!  Fix this!
        cmd = 'char-write-cmd 0x%02x 0%x' % (handle, value)
        self.con.sendline( cmd )
        return
    
    def readHandler( self, handle ):
        self.con.sendline('char-read-hnd 0x%02x' % handle)
        self.con.expect('descriptor: .* \r')
        after = self.con.after
        rval = after.split()[1:]
        try:
            val = [long(float.fromhex(n)) for n in rval]
        except:
            val = [long(0) for n in rval]
        pass
        return val

    def mainLoop( self,barCalibration ):
        i=0
        storage = MeasurementStorage()
        while True:
            self.con.expect('Notification handle = .* \r')
            hxstr = self.con.after.split()[3:]
            #print hxstr[0],  [long(float.fromhex(n)) for n in hxstr[2:]]
            handle = long(float.fromhex(hxstr[0]))
            try:
                self.cb[handle](storage,[long(float.fromhex(n)) for n in hxstr[2:]])
                i=i+1
            except: 
                print "Error in callback for %x" % handle
            pass

            try:
                storage.sendIfAllSet()
            except:
                print 'Error in sending measurement'
            pass

            if i > 5:
                barometer(self,barCalibration,storage)
                i=0
        pass
    
    def registerHandler( self, handle, fn ):
        self.cb[handle]=fn;
        return

def temperature(storage,v):
    objT = (v[1]<<8)+v[0]
    ambT = (v[3]<<8)+v[2]
    temperature = calculateTemperature(objT, ambT)
    storage.setTemperature(temperature)
    print "1> TEMPERATUTE IR (Celcius) %.1f" % temperature

def acceleration(storage,v):
    (x,y,z,mag) = calculateAcceleration(v[0],v[1],v[2])
    storage.setAcceleration(x,y,z,mag)
    print "2> ACCELEROMETER XYZ", x,y,z
    print "2> ACCELEROMETER M (m/sec2)", mag

def magnetometer(storage,v):
    x = (v[1]<<8)+v[0]
    y = (v[3]<<8)+v[2]
    z = (v[5]<<8)+v[4]
    (magX,magY,magZ) = calculateMagneticForce(x,y,z)
    storage.setMagneticForce(magX,magY,magZ)
    print "3> MAGNETOMETER (uTera)", (magX,magY,magZ)

def gyroscope(storage,v):
    x = (v[1]<<8)+v[0]
    y = (v[3]<<8)+v[2]
    z = (v[5]<<8)+v[4]
    (degX,degY,degZ) = calculateGyroscopeRotation(x,y,z)
    storage.setGyroRotation(degX,degY,degZ)
    print "4> GYROSCOPE (deg/sec)", (degX,degY,degZ)

def humidity(storage,v):
    rawT = (v[1]<<8)+v[0]
    rawH = (v[3]<<8)+v[2]
    (t,rh) = calculateHumidity(rawT,rawH)
    storage.setHumidity(rh)
    #print "5> TEMPERATURE H (C)", t
    print "5> HUMIDITY (1/M3)", rh  

def barometer(tag,barCalibration,storage):
    v = tag.readHandler(0x4B)
    tRaw = (v[1]<<8)+v[0]
    pRaw = (v[3]<<8)+v[2]
    #barTemp = calculateBarometerTemperature(tRaw,barCalibration)
    #print '6> PRESSURE (T)',barTemp
    barPressure = calculateBarometerPressure(pRaw,tRaw,barCalibration)
    storage.setPressure(barPressure)
    print '6> PRESSURE (hPa)',barPressure

def main():
    global SensorID,url,lat,lon
    SensorID = sys.argv[1]
    tag = TISensorTag(sys.argv[2]) #'BC:6A:29:C3:46:11'
    lat = sys.argv[3]
    lon = sys.argv[4]
    url = sys.argv[5] #"http://localhost/testing/posting.php"
    # enable temperature sensor
    tag.registerHandler(0x25,temperature)
    tag.writeCommand(0x29,0x01)
    tag.writeCommand(0x26,0x0100)

    # enable accelerometer
    tag.registerHandler(0x2d,acceleration)
    tag.writeCommand(0x31,0x01)
    tag.writeCommand(0x2e,0x0100)

    # enable magnetometer
    tag.registerHandler(0x40,magnetometer)
    tag.writeCommand(0x44,0x01)
    tag.writeCommand(0x41,0x0100)

    # enable gyroscope
    tag.registerHandler(0x57,gyroscope)
    tag.writeCommand(0x5B,0x07)
    tag.writeCommand(0x58,0x0100)

    #enable humidity sensor
    tag.registerHandler(0x38,humidity)
    tag.writeCommand(0x3C,0x01)
    tag.writeCommand(0x39,0x0100)

    #read barometer calibration
    tag.writeCommand(0x4F,0x02)
    v = tag.readHandler(0x52)
    #store it
    barCalibration = BarometerCalibration(v)
    #enable barometer
    tag.writeCommand(0x4F,0x01)

    tag.mainLoop(barCalibration)

if __name__ == "__main__":
    main()

