namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Enums
{
    //Enums compatibility considerations:
    //Beggining with '_' must be removed
    //Doble underscore '__' must be replaced by '-'
    public enum Codec
    {
        Default,
        copy,                                //Just change the container not the codec

        //VIDEO

        amv,                                 //AMV Video
        apng,                                //APNG (Animated Portable Network Graphics) image      
        bmp,                                 //BMP (Windows and OS/2 bitmap)
        cinepak,                             //Cinepak     
        dnxhd,                               //VC3/DNxHD
        dvvideo,                             //DV (Digital Video)
        ffv1,                                //FFmpeg video codec #1
        flashsv,                             //Flash Screen Video
        flashsv2,                            //Flash Screen Video Version 2
        flv,                                 //FLV / Sorenson Spark / Sorenson H.263 (Flash Video) (codec flv1)
        h261,                                //H.261
        h263,                                //H.263 / H.263-1996
        h263p,                               //H.263+ / H.263-1998 / H.263 version 2
        libx264,                             //libx264 H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (codec h264)
        libx264rgb,                          //libx264 H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 RGB (codec h264)
        h264_amf,                            //AMD AMF H.264 Encoder (codec h264)
        h264_nvenc,                          //NVIDIA NVENC H.264 encoder (codec h264)
        h264_qsv,                            //H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (Intel Quick Sync Video acceleration) (codec h264)
        nvenc,                               //NVIDIA NVENC H.264 encoder (codec h264)
        nvenc_h264,                          //NVIDIA NVENC H.264 encoder (codec h264)      
        libx265,                             //libx265 H.265 / HEVC (codec hevc)
        nvenc_hevc,                          //NVIDIA NVENC hevc encoder (codec hevc)
        hevc_amf,                            //AMD AMF HEVC encoder (codec hevc)
        hevc_nvenc,                          //NVIDIA NVENC hevc encoder (codec hevc)
        hevc_qsv,                            //HEVC (Intel Quick Sync Video acceleration) (codec hevc)
        jpeg2000,                            //JPEG 2000
        libopenjpeg,                         //OpenJPEG JPEG 2000 (codec jpeg2000)
        jpegls,                              //JPEG-LS
        ljpeg,                               //Lossless JPEG
        mjpeg,                               //MJPEG (Motion JPEG)
        mjpeg_qsv,                           //MJPEG (Intel Quick Sync Video acceleration) (codec mjpeg)
        mpeg1video,                          //MPEG-1 video
        mpeg2video,                          //MPEG-2 video
        mpeg2_qsv,                           //MPEG-2 video (Intel Quick Sync Video acceleration) (codec mpeg2video)
        mpeg4,                               //MPEG-4 part 2
        libxvid,                             //libxvidcore MPEG-4 part 2 (codec mpeg4)      
        pbm,                                 //PBM (Portable BitMap) image
        pcx,                                 //PC Paintbrush PCX image
        pgm,                                 //PGM (Portable GrayMap) image
        png,                                 //PNG (Portable Network Graphics) image
        ppm,                                 //PPM (Portable PixelMap) image
        prores,                              //Apple ProRes
        prores_aw,                           //Apple ProRes (codec prores)
        prores_ks,                           //Apple ProRes (iCodec Pro) (codec prores)
        qtrle,                               //QuickTime Animation (RLE) video
        rawvideo,                            //raw video
        roqvideo,                            //id RoQ video (codec roq) 
        tiff,                                //TIFF image
        libvpx,                              //libvpx VP8 (codec vp8)
        libvpx__vp9,                         //libvpx VP9 (codec vp9)
        vp9_qsv,                             //VP9 video (Intel Quick Sync Video acceleration) (codec vp9)
        libwebp_anim,                        //libwebp WebP image (codec webp)
        libwebp,                             //libwebp WebP image (codec webp)
        wmv1,                                //Windows Media Video 7
        wmv2,                                //Windows Media Video 8
        zlib,                                //LCL (LossLess Codec Library) ZLIB
        zmbv,                                //Zip Motion Blocks Video,
        libtheora,                           //Xiph - (Lossy)

        // AUDIO

        libmp3lame,                          //MPEG Audi Layer 3
        aax,                                 //Audible Enhanced Audiobook
        aaxc,                                //Audible Enhanced Audiobook v2
        aac,                                 //Advanced Audio Coding
        libfdk_aac,                          //Very goog AAC encoder
        ac3,                                 //ATSC A/52A (AC-3)
        ac3_fixed,                           //ATSC A/52A (AC-3) (codec ac3)
        alac,                                //ALAC (Apple Lossless Audio Codec)
        flac,                                //Free Loseless Audio Codec
        mp2,                                 //MP2 (MPEG audio layer 2)
        mp2fixed,                            //MP2 fixed point (MPEG audio layer 2) (codec mp2)
        wmav1,                               //Window Media Audio 1
        wmav2,                               //Windows Media Audio 2   
        truehd,                              //TrueHD
        vorbis,                              //Vorbis
        libvorbis,                           //libvorbis (codec vorbis)
        wavpack,                             //WavPack
        pcm_u8,                              //Wave,
        opus,                                //Opus,
        real_144                             //RealPlayer audi
    }

    //Enums compatibility considerations:
    //Beggining with '_' must be removed
    //Doble underscore '__' must be replaced by '-'
    public enum VideoCodec
    {
        amv,                                 //AMV Video
        apng,                                //APNG (Animated Portable Network Graphics) image      
        bmp,                                 //BMP (Windows and OS/2 bitmap)
        cinepak,                             //Cinepak     
        dnxhd,                               //VC3/DNxHD
        dvvideo,                             //DV (Digital Video)
        ffv1,                                //FFmpeg video codec #1
        flashsv,                             //Flash Screen Video
        flashsv2,                            //Flash Screen Video Version 2
        flv,                                 //FLV / Sorenson Spark / Sorenson H.263 (Flash Video) (codec flv1)
        h261,                                //H.261
        h263,                                //H.263 / H.263-1996
        h263p,                               //H.263+ / H.263-1998 / H.263 version 2
        libx264,                             //libx264 H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (codec h264)
        libx264rgb,                          //libx264 H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 RGB (codec h264)
        h264_amf,                            //AMD AMF H.264 Encoder (codec h264)
        h264_nvenc,                          //NVIDIA NVENC H.264 encoder (codec h264)
        h264_qsv,                            //H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (Intel Quick Sync Video acceleration) (codec h264)
        nvenc,                               //NVIDIA NVENC H.264 encoder (codec h264)
        nvenc_h264,                          //NVIDIA NVENC H.264 encoder (codec h264)      
        libx265,                             //libx265 H.265 / HEVC (codec hevc)
        nvenc_hevc,                          //NVIDIA NVENC hevc encoder (codec hevc)
        hevc_amf,                            //AMD AMF HEVC encoder (codec hevc)
        hevc_nvenc,                          //NVIDIA NVENC hevc encoder (codec hevc)
        hevc_qsv,                            //HEVC (Intel Quick Sync Video acceleration) (codec hevc)
        jpeg2000,                            //JPEG 2000
        libopenjpeg,                         //OpenJPEG JPEG 2000 (codec jpeg2000)
        jpegls,                              //JPEG-LS
        ljpeg,                               //Lossless JPEG
        mjpeg,                               //MJPEG (Motion JPEG)
        mjpeg_qsv,                           //MJPEG (Intel Quick Sync Video acceleration) (codec mjpeg)
        mpeg1video,                          //MPEG-1 video
        mpeg2video,                          //MPEG-2 video
        mpeg2_qsv,                           //MPEG-2 video (Intel Quick Sync Video acceleration) (codec mpeg2video)
        mpeg4,                               //MPEG-4 part 2
        libxvid,                             //libxvidcore MPEG-4 part 2 (codec mpeg4)      
        pbm,                                 //PBM (Portable BitMap) image
        pcx,                                 //PC Paintbrush PCX image
        pgm,                                 //PGM (Portable GrayMap) image
        png,                                 //PNG (Portable Network Graphics) image
        ppm,                                 //PPM (Portable PixelMap) image
        prores,                              //Apple ProRes
        prores_aw,                           //Apple ProRes (codec prores)
        prores_ks,                           //Apple ProRes (iCodec Pro) (codec prores)
        qtrle,                               //QuickTime Animation (RLE) video
        rawvideo,                            //raw video
        roqvideo,                            //id RoQ video (codec roq) 
        tiff,                                //TIFF image
        libvpx,                              //libvpx VP8 (codec vp8)
        libvpx__vp9,                         //libvpx VP9 (codec vp9)
        vp9_qsv,                             //VP9 video (Intel Quick Sync Video acceleration) (codec vp9)
        libwebp_anim,                        //libwebp WebP image (codec webp)
        libwebp,                             //libwebp WebP image (codec webp)
        wmv1,                                //Windows Media Video 7
        wmv2,                                //Windows Media Video 8
        zlib,                                //LCL (LossLess Codec Library) ZLIB
        zmbv,                                //Zip Motion Blocks Video,
        libtheora                            //Xiph - (Lossy)
    }

    //Enums compatibility considerations:
    //Beggining with '_' must be removed
    //Doble underscore '__' must be replaced by '-'
    public enum AudioCodec
    {
        libmp3lame,                          //MPEG Audi Layer 3
        aax,                                 //Audible Enhanced Audiobook
        aaxc,                                 //Audible Enhanced Audiobook
        aac,                                 //Advanced Audio Coding
        libfdk_aac,                          //Very goog AAC encoder
        ac3,                                 //ATSC A/52A (AC-3)
        ac3_fixed,                           //ATSC A/52A (AC-3) (codec ac3)
        alac,                                //ALAC (Apple Lossless Audio Codec)
        flac,                                //Free Loseless Audio Codec
        mp2,                                 //MP2 (MPEG audio layer 2)
        mp2fixed,                            //MP2 fixed point (MPEG audio layer 2) (codec mp2)
        wmav1,                               //Window Media Audio 1
        wmav2,                               //Windows Media Audio 2    
        truehd,                              //TrueHD
        vorbis,                              //Vorbis
        libvorbis,                           //libvorbis (codec vorbis)
        wavpack,                             //WavPack
        pcm_u8,                              //Wave,
        opus,                                //Opus
        real_144                             //RealPlayer audi
    }

    public enum FormatType
    {
        Audio,
        Video
    }

    public enum HardwareEncoderType
    {
        amf,
        nvenc,
        qsv,
        none
    }
}