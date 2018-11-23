extern "C" {
	//iOS平台上的原生LOG
	void nativeLog(const char* debugMessage){
		
		NSLog(@"Received _logToiOS %@", [NSString stringWithUTF8String:debugMessage]);
	}
}