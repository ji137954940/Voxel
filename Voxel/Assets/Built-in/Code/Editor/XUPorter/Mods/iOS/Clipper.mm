//

//  Clipper.m

//  Clip

//

#if defined(__cplusplus)
extern "C" {
    extern NSString* CreateNSString (const char* string);
#endif

char *MakeStringCpy(const char* string);



void SetTextIOS(const char* c){

    [UIPasteboard generalPasteboard].string = [NSString stringWithCString: c encoding:NSUTF8StringEncoding];

}



char *GetTextIOS(){

    return MakeStringCpy([[UIPasteboard generalPasteboard].string UTF8String]);

}



char *MakeStringCpy(const char* string){

   	if (string == NULL)

        return NULL;

    

    char* res = (char*)malloc(strlen(string) + 1);

    strcpy(res, string);

    return res;

}

void changeBrightness(const char *result)

{

	//只对iPhone设备处理

	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) {

		float f = [CreateNSString(result) floatValue] / 10;

//            float value = f < 1 ? f : [[AsSDKConnector shareAsInstance] brightness];

		[[UIScreen mainScreen] setBrightness:f];

	}

}
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

#if defined(__cplusplus)
}
#endif
