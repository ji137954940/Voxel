﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Color.Number.Animation
{

    public partial class GifAnimation
    {
        /// <summary>
        /// gif 动画信息
        /// </summary>
        public class GifInfo
        {
            //public int loopCount;
            /// <summary>
            /// 宽
            /// </summary>
            public int Width;

            /// <summary>
            /// 高
            /// </summary>
            public int Height;

            /// <summary>
            /// gif texture 图片列表
            /// </summary>
            public List<GifTexture> TextureList;
        }

        /// <summary>
        /// gif  单个 图片信息
        /// </summary>
        public struct GifTexture
        {
            /// <summary>
            /// texture 图片
            /// </summary>
            public Texture2D Texture2D;

            /// <summary>
            /// texture 图片播放持续时间
            /// </summary>
            public float DelaySec;

            /// <summary>
            /// 初始化单张 gif texture 图片信息
            /// </summary>
            /// <param name="texture2D"></param>
            /// <param name="delaySec"></param>
            public GifTexture(Texture2D texture2D, float delaySec)
            {
                this.Texture2D = texture2D;
                this.DelaySec = delaySec;
            }
        }

        /// <summary>
        /// GIF Data Format
        /// </summary>
        public struct GifData
        {
            // Signature
            public byte sig0, sig1, sig2;

            // Version
            public byte ver0, ver1, ver2;

            // Logical Screen Width
            public ushort logicalScreenWidth;

            // Logical Screen Height
            public ushort logicalScreenHeight;

            // Global Color Table Flag
            public bool globalColorTableFlag;

            // Color Resolution
            public int colorResolution;

            // Sort Flag
            public bool sortFlag;

            // Size of Global Color Table
            public int sizeOfGlobalColorTable;

            // Background Color Index
            public byte bgColorIndex;

            // Pixel Aspect Ratio
            public byte pixelAspectRatio;

            // Global Color Table
            public List<byte[]> globalColorTable;

            // ImageBlock
            public List<ImageBlock> imageBlockList;

            // GraphicControlExtension
            public List<GraphicControlExtension> graphicCtrlExList;

            // Comment Extension
            public List<CommentExtension> commentExList;

            // Plain Text Extension
            public List<PlainTextExtension> plainTextExList;

            // Application Extension
            public ApplicationExtension appEx;

            // Trailer
            public byte trailer;

            public string signature
            {
                get
                {
                    char[] c = {(char) sig0, (char) sig1, (char) sig2};
                    return new string(c);
                }
            }

            public string version
            {
                get
                {
                    char[] c = {(char) ver0, (char) ver1, (char) ver2};
                    return new string(c);
                }
            }

            /// <summary>
            /// 输出记录 图片信息
            /// </summary>
            public void Dump()
            {
                Debug.Log("GIF Type: " + signature + "-" + version);
                Debug.Log("Image Size: " + logicalScreenWidth + "x" + logicalScreenHeight);
                Debug.Log("Animation Image Count: " + imageBlockList.Count);
                Debug.Log("Animation Loop Count (0 is infinite): " + appEx.loopCount);
                Debug.Log("Application Identifier: " + appEx.applicationIdentifier);
                Debug.Log("Application Authentication Code: " + appEx.applicationAuthenticationCode);
            }
        }

        /// <summary>
        /// Image Block
        /// </summary>
        public struct ImageBlock
        {
            // Image Separator
            public byte imageSeparator;

            // Image Left Position
            public ushort imageLeftPosition;

            // Image Top Position
            public ushort imageTopPosition;

            // Image Width
            public ushort imageWidth;

            // Image Height
            public ushort imageHeight;

            // Local Color Table Flag
            public bool localColorTableFlag;

            // Interlace Flag
            public bool interlaceFlag;

            // Sort Flag
            public bool sortFlag;

            // Size of Local Color Table
            public int sizeOfLocalColorTable;

            // Local Color Table
            public List<byte[]> localColorTable;

            // LZW Minimum Code Size
            public byte LzwMinimumCodeSize;

            // Block Size & Image Data List
            public List<ImageDataBlock> imageDataList;

            public struct ImageDataBlock
            {
                // Block Size
                public byte blockSize;

                // Image Data
                public byte[] imageData;
            }
        }

        /// <summary>
        /// Graphic Control Extension
        /// </summary>
        public struct GraphicControlExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;

            // Graphic Control Label
            public byte graphicControlLabel;

            // Block Size
            public byte blockSize;

            // Disposal Mothod
            public ushort disposalMethod;

            // Transparent Color Flag
            public bool transparentColorFlag;

            // Delay Time
            public ushort delayTime;

            // Transparent Color Index
            public byte transparentColorIndex;

            // Block Terminator
            public byte blockTerminator;
        }

        /// <summary>
        /// Comment Extension
        /// </summary>
        public struct CommentExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;

            // Comment Label
            public byte commentLabel;

            // Block Size & Comment Data List
            public List<CommentDataBlock> commentDataList;

            public struct CommentDataBlock
            {
                // Block Size
                public byte blockSize;

                // Image Data
                public byte[] commentData;
            }
        }

        /// <summary>
        /// Plain Text Extension
        /// </summary>
        public struct PlainTextExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;

            // Plain Text Label
            public byte plainTextLabel;

            // Block Size
            public byte blockSize;

            // Block Size & Plain Text Data List
            public List<PlainTextDataBlock> plainTextDataList;

            public struct PlainTextDataBlock
            {
                // Block Size
                public byte blockSize;

                // Plain Text Data
                public byte[] plainTextData;
            }
        }

        /// <summary>
        /// Application Extension
        /// </summary>
        public struct ApplicationExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;

            // Extension Label
            public byte extensionLabel;

            // Block Size
            public byte blockSize;

            // Application Identifier
            public byte appId1, appId2, appId3, appId4, appId5, appId6, appId7, appId8;

            // Application Authentication Code
            public byte appAuthCode1, appAuthCode2, appAuthCode3;

            // Block Size & Application Data List
            public List<ApplicationDataBlock> appDataList;

            public struct ApplicationDataBlock
            {
                // Block Size
                public byte blockSize;

                // Application Data
                public byte[] applicationData;
            }

            public string applicationIdentifier
            {
                get
                {
                    char[] c =
                    {
                        (char) appId1, (char) appId2, (char) appId3, (char) appId4, (char) appId5, (char) appId6,
                        (char) appId7, (char) appId8
                    };
                    return new string(c);
                }
            }

            public string applicationAuthenticationCode
            {
                get
                {
                    char[] c = {(char) appAuthCode1, (char) appAuthCode2, (char) appAuthCode3};
                    return new string(c);
                }
            }

            public int loopCount
            {
                get
                {
                    if (appDataList == null || appDataList.Count < 1 ||
                        appDataList[0].applicationData.Length < 3 ||
                        appDataList[0].applicationData[0] != 0x01)
                    {
                        return 0;
                    }

                    return BitConverter.ToUInt16(appDataList[0].applicationData, 1);
                }
            }
        }
    }
}

