﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPF.FlipView
{
    public class AnimationFactory
    {
        public static AnimationFactory Instance
        {
            get
            {
                return new AnimationFactory();
            }
        }

        public Storyboard GetAnimation(DependencyObject target, double to, double from)
        {
            Storyboard story = new Storyboard();
            Storyboard.SetTargetProperty(story, new PropertyPath("(TextBlock.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTarget(story, target);

            var doubleAnimation = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame fromFrame = new EasingDoubleKeyFrame(from);
            fromFrame.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
            fromFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));

            EasingDoubleKeyFrame toFrame = new EasingDoubleKeyFrame(to);
            toFrame.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
            toFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(210));

            doubleAnimation.KeyFrames.Add(fromFrame);
            doubleAnimation.KeyFrames.Add(toFrame);
            story.Children.Add(doubleAnimation);

            return story;
        }
    }
}
