﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BingHelper;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Wimi
{
    public partial class MainPage : Page
    {
        private bool _isLocked = true;
        public async Task InitLockScreenAsync()
        {
            gridCommand.Background = new SolidColorBrush(Colors.Black);

            BingImage bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            if(bingUnit != null)
            {
                imageLockScreen.Source = new BitmapImage(new Uri(bingUnit.Path));
            }
        }

        public async Task LockScreenAsync()
        {
            if (_isLocked)
            {
                return;
            }
            _isLocked = true;
            gridCommand.Background = new SolidColorBrush(Colors.Black);
            await gridLockScreen.Offset(offsetX: 0, offsetY: -(float)Window.Current.Bounds.Height, duration: 0, delay: 0, easingType: EasingType.Linear).StartAsync();
            //gridLockScreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            await InitLockScreenAsync();
            SetVoice("lock.mp3", true);
            await gridLockScreen.Offset(offsetX: 0, offsetY: 0, duration: 2000, delay: 50, easingType: EasingType.Quintic).StartAsync();
            WimiClose();
        }

        public async Task UnlockScreenAsync()
        {
            if (!_isLocked)
            {
                return;
            }
            _isLocked = false;
            gridCommand.Background = new SolidColorBrush(Colors.Transparent);
            SetVoice("unlock.mp3", true);
            await gridLockScreen.Offset(offsetX: 0, offsetY: -(float)Window.Current.Bounds.Height, duration: 2000, delay: 50, easingType: EasingType.Quintic).StartAsync();
        }
    }
}
