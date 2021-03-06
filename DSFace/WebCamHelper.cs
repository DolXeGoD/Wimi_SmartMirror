﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace DSFace
{

    public class WebcamHelper
    {
        public MediaCapture mediaCapture;
        public VideoEncodingProperties videoProperties;

        private bool initialized = false;

        /// <summary>
        /// Asynchronously initializes webcam feed
        /// </summary>
        public async Task<bool> InitializeCameraAsync()
        {
            if (mediaCapture == null)
            {
                // Attempt to get attached webcam
                var cameraDevice = await FindCameraDevice();

                if (cameraDevice == null)
                {
                    // No camera found, report the error and break out of initialization
                    Debug.WriteLine("No camera found!");
                    initialized = false;
                    return false;
                }

                // Creates MediaCapture initialization settings with foudnd webcam device
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync(settings);

                mediaCapture.Failed += MediaCapture_Failed;
                mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                initialized = true;

                this.videoProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                return true;
            }
            return true;
        }

        private void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            Debug.WriteLine("MediaCapture_RecordLimitationExceeded");
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed");
        }

        /// <summary>
        /// Asynchronously looks for and returns first camera device found.
        /// If no device is found, return null
        /// </summary>
        private static async Task<DeviceInformation> FindCameraDevice()
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


            if (allVideoDevices.Count > 0)
            {
                // If there is a device attached, return the first device found
                return allVideoDevices[0];
            }
            else
            {
                // Else, return null
                return null;
            }
        }

        /// <summary>
        /// Asynchronously begins live webcam feed
        /// </summary>
        public async Task StartCameraPreview()
        {
            try
            {
                await mediaCapture.StartPreviewAsync();
            }
            catch
            {
                initialized = false;
                Debug.WriteLine("Failed to start camera preview stream");

            }
        }

        /// <summary>
        /// Asynchronously ends live webcam feed
        /// </summary>
        public async Task StopCameraPreview()
        {
            try
            {
                await mediaCapture.StopPreviewAsync();
            }
            catch
            {
                Debug.WriteLine("Failed to stop camera preview stream");
            }
        }


        /// <summary>
        /// Asynchronously captures photo from camera feed and stores it in local storage. Returns image file as a StorageFile.
        /// File is stored in a temporary folder and could be deleted by the system at any time.
        /// </summary>
        public async Task<StorageFile> CapturePhoto()
        {
            // Create storage file in local app storage
            string fileName = GenerateNewFileName() + ".jpg";
            CreationCollisionOption collisionOption = CreationCollisionOption.GenerateUniqueName;
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, collisionOption);

            // Captures and stores new Jpeg image file
            try
            {
                await mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);
            }
            catch(Exception e)
            {
                Debug.WriteLine("CapturePhoto - " + e.Message);
                return null;
            }

            // Return image file
            return file;
        }

        /// <summary>
        /// Generates unique file name based on current time and date. Returns value as string.
        /// </summary>
        private string GenerateNewFileName()
        {
            return DateTime.UtcNow.ToString("yyyy.MMM.dd HH-mm-ss") + " Wimi Face";
        }

        /// <summary>
        /// Returns true if webcam has been successfully initialized. Otherwise, returns false.
        /// </summary>
        public bool IsInitialized()
        {
            return initialized;
        }
    }

}
