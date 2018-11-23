

#import "PhotoManager.h"
@implementation PhotoManager
- ( void ) imageSaved: ( UIImage *) image didFinishSavingWithError:( NSError *)error 
    contextInfo: ( void *) contextInfo
{
    
    if (error != nil)
    {
        NSLog(@"����ʧ��");
    }
    else
    {
	NSLog(@"����ɹ�");
    }
}

void _SavePhoto(char *readAddr)
{
    NSString *strReadAddr = [NSString stringWithUTF8String:readAddr];
    UIImage *img = [UIImage imageWithContentsOfFile:strReadAddr];
    NSLog([NSString stringWithFormat:@"w:%f, h:%f", img.size.width, img.size.height]);
    PhotoManager *instance = [PhotoManager alloc];
    UIImageWriteToSavedPhotosAlbum(img, instance, 
        @selector(imageSaved:didFinishSavingWithError:contextInfo:), nil);
}


void _SaveVideo(char *path){
    NSLog(@"������Ƶ��ַ%s-------------",path);
    if (path == NULL) {
        return;
    }
    NSString *videoPath = [NSString stringWithUTF8String:path];
    ALAssetsLibrary *library = [[ALAssetsLibrary alloc] init];
    [library writeVideoAtPathToSavedPhotosAlbum:[NSURL fileURLWithPath:videoPath]
                                completionBlock:^(NSURL *assetURL, NSError *error) {
                                    if (error) {
                                        NSLog(@"Save video fail:%@",error);
                                    } else {
                                        NSLog(@"Save video succeed.");
                                    }
                                }];

}

@end



