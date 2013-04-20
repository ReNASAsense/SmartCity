#pragma pack(push)  /* push current alignment to stack */
#pragma pack(1)     /* set alignment to 1 byte boundary */

struct SensorReading
{
	// definition how to parse received data
	int sensor_type;
	// data in the sensor buffer
	int data_length;
	char sensor_data_checksum;
	char sensor_data_reserve1;
	char sensor_data_reserve2;
	char sensor_data_reserve3;
	// data
	char sensor_data[];	
}

#pragma pack(pop)   /* restore original alignment from stack */