#!/usr/bin/env python
import pexpect
import sys
import time
import json
import requests

class Measure:
    def __init__(self):
    self.con = pexpect.spawn('minicom -b 9600 -o -D /dev/ttyAMA0')

    def sendMeasurements(self,battery,wind):
        dataToSend = "<Measurements><Measurement>\
                      <deviceId>"+str(SensorID)+"</deviceId>\
                      <latitude>"+str(lat)+"</latitude>\
                      <longitude>"+str(lon)+"</longitude>\
                      <battery>"+str(battery)+"</battery>\
                      <wind>"+str(wind)+"</windbattery>\
                      </Measurement></Measurements>"
        print 'Sending measurement'  
        r = requests.post(url, data=dataToSend)
        print r


    def mainLoop(self):  
        while True:
            try:
        self.con.expect('H.*')
                after = (self.con.after.split())[0]
        values = after[1:]
        print values
        windR = values[2:]
              batteryR = values[:-2]
        battery = int(batteryR.encode('hex'), 16)
        wind = int(windR.encode('hex'),16)
        battery = long(5.3 * battery / 0.365)
        print 'Wind ' , wind
        print 'Battery ' , battery
        self.sendMeasurements(battery,wind)
            except: 
                print "Error"
            pass
        pass

def main():
    global SensorID,url,lat,lon
    SensorID = sys.argv[1]
    lat = sys.argv[2]
    lon = sys.argv[3]
    url = sys.argv[4] #"http://localhost/testing/posting.php"
    measurer = Measure()
    measurer.mainLoop()

if __name__ == "__main__":
    main()