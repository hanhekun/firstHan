using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.FlipView
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPF.FlipView"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPF.FlipView;assembly=WPF.FlipView"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:FlipView/>
    ///
    /// </summary>
    public class FlipView : Selector
    {
        #region Private Fields
        private ContentControl PART_CurrentItem;
        private ContentControl PART_PreviousItem;
        private ContentControl PART_First;
        private ContentControl PART_Last;
        private ContentControl PART_Left;
        private ContentControl PART3;
        private ContentControl PART_3;
        private ContentControl PART4;
        private ContentControl PART_4;
        private ContentControl PART5;
        private ContentControl PART_5;

        private ContentControl PART_Right;
        private ContentControl PART_NextItem;
        private FrameworkElement PART_Root;
        private FrameworkElement PART_Container;
        private double fromValue = 0.0;
        private double elasticFactor = 1.0;
        private Point myPoint;
        private int n=0;
        #endregion

        #region Constructor
        static FlipView()
        {
           
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
            SelectedIndexProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));            
        }

        public FlipView()
        {
            n = 0;
            this.CommandBindings.Add(new CommandBinding(NextCommand, this.OnNextExecuted, this.OnNextCanExecute));
            this.CommandBindings.Add(new CommandBinding(PreviousCommand, this.OnPreviousExecuted, this.OnPreviousCanExecute));
        }
        #endregion

        #region Private methods
        private void OnRootManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.fromValue = e.TotalManipulation.Translation.X;
            var offsetX = this.fromValue;
            var a = this.ActualWidth;

            if (this.SelectedIndex == 0 && offsetX > 0)
            {
                this.SelectedIndex = 0;
                this.RunSlideAnimation(0, ((MatrixTransform)this.PART_Root.RenderTransform).Matrix.OffsetX);
            }
            if (this.SelectedIndex == this.Items.Count-1 && offsetX < 0)
            {
                this.SelectedIndex = this.Items.Count-1;
                this.RunSlideAnimation(0, ((MatrixTransform)this.PART_Root.RenderTransform).Matrix.OffsetX);
            }

            if (offsetX > a / 10)
            {
                if (offsetX < 3 * a / 10 && this.SelectedIndex > 0)
                {
                    this.SelectedIndex -= 1;
                }
                if (offsetX > 3 * a / 10 && this.SelectedIndex > 0)
                {
                    if (this.SelectedIndex > 1)
                    {
                        if(offsetX > 5 * a / 10)
                        {
                            if (this.SelectedIndex > 2)
                            {
                                if (offsetX > 7 * a / 10)
                                {
                                    if (this.SelectedIndex > 3)
                                    {
                                        if (offsetX > 9 * a / 10)
                                        {
                                            if(this.SelectedIndex > 4)
                                            {
                                                this.SelectedIndex -= 5;
                                            }
                                            else
                                            {
                                                this.SelectedIndex -= 4;
                                            }
                                        }
                                        else
                                        {
                                            this.SelectedIndex -= 4;
                                        }

                                    }
                                    else
                                    {
                                        this.SelectedIndex -= 3;
                                    }
                                    
                                }
                                else
                                {
                                    this.SelectedIndex -= 3;
                                }

                            }
                            else
                            {
                                this.SelectedIndex -= 2;
                            }
                                
                        }
                        else
                        {
                            this.SelectedIndex -= 2;
                        }

                    }
                    else
                    {
                        this.SelectedIndex -= 1;
                    }

                }
            }
            else if (offsetX < -a / 10)
            {
                if (offsetX > -3 * a / 10 && (this.SelectedIndex < this.Items.Count))
                {
                    this.SelectedIndex += 1;
                }
                if (offsetX < -3 * a / 10 && (this.SelectedIndex < this.Items.Count))
                {

                    if (this.SelectedIndex < this.Items.Count - 2)
                    {
                        if(offsetX < -5 * a / 10)
                        {
                            if(this.SelectedIndex < this.Items.Count - 3)
                            {
                                if (offsetX < -7 * a / 10)
                                {
                                    if (this.SelectedIndex < this.Items.Count - 4)
                                    {
                                        if (offsetX < -9 * a / 10)
                                        {
                                            if(this.SelectedIndex < this.Items.Count - 5)
                                            {
                                                this.SelectedIndex = this.SelectedIndex + 5;

                                            }
                                            else
                                            {
                                                this.SelectedIndex = this.SelectedIndex + 4;
                                            }
                                        }
                                        else
                                        {
                                            this.SelectedIndex = this.SelectedIndex + 4;
                                        }
                                    }
                                    else
                                    {
                                       
                                            this.SelectedIndex = this.SelectedIndex + 3;
                                    }
                                   
                                }
                                else
                                {
                                    this.SelectedIndex = this.SelectedIndex + 3;
                                }

                            }
                            else
                            {
                                this.SelectedIndex = this.SelectedIndex + 2;
                            }
                        }
                        else
                        {
                            this.SelectedIndex+=2;
                        }
                        
                    }
                    else
                    {
                        this.SelectedIndex++;
                        //n = 1;
                    }
                }

            }
            else
            {
                if (this.fromValue != 0)
                {
                    try
                    {
                        this.RunSlideAnimation(0, ((MatrixTransform)this.PART_Root.RenderTransform).Matrix.OffsetX);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }



            if (this.elasticFactor < 1)
            {
                try
                {
                    this.RunSlideAnimation(0, ((MatrixTransform)this.PART_Root.RenderTransform).Matrix.OffsetX);
                    
                }catch(Exception ex)
                {

                }
            }
            this.elasticFactor = 1.0;
        }
        
        private void OnRootManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!(this.PART_Root.RenderTransform is MatrixTransform))
            {
                this.PART_Root.RenderTransform = new MatrixTransform();
            }

            Matrix matrix = ((MatrixTransform)this.PART_Root.RenderTransform).Matrix;
            var delta = e.DeltaManipulation;

            if ((this.SelectedIndex == 0 && delta.Translation.X > 0 && this.elasticFactor > 0)
                || (this.SelectedIndex == this.Items.Count - 1 && delta.Translation.X < 0 && this.elasticFactor > 0))
            {
                //this.elasticFactor -= 0.05;
            }

            matrix.Translate(delta.Translation.X * elasticFactor, 0);
            this.PART_Root.RenderTransform = new MatrixTransform(matrix);

            e.Handled = true;
        }

        private void OnRootManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this.PART_Container;
            e.Handled = true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshViewPort(this.SelectedIndex);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.SelectedIndex > -1)
            {
                this.RefreshViewPort(this.SelectedIndex);
            }
        }
        public static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlipView;
            
            control.OnSelectedIndexChanged(e);
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }

            if ((int)e.NewValue >= 0 && (int)e.NewValue < this.Items.Count)
            {
                n = Math.Abs(int.Parse(e.NewValue.ToString()) - int.Parse(e.OldValue.ToString()));
                double toValue = (int)e.OldValue < (int)e.NewValue ? -this.ActualWidth * n / 5 : this.ActualWidth * n / 5;
                this.RunSlideAnimation(toValue, fromValue);                
            }
        }

        private void RefreshViewPort(int selectedIndex)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }
            Canvas.SetLeft(this.PART_5, -4 * this.ActualWidth / 5);
            Canvas.SetLeft(this.PART5, 8 * this.ActualWidth / 5);
            Canvas.SetLeft(this.PART_4, -3 * this.ActualWidth / 5);
            Canvas.SetLeft(this.PART4, 7 * this.ActualWidth / 5);
            Canvas.SetLeft(this.PART_3, -2*this.ActualWidth / 5);
            Canvas.SetLeft(this.PART3, 6*this.ActualWidth /5 );
            Canvas.SetLeft(this.PART_Left, -this.ActualWidth / 5);
            Canvas.SetLeft(this.PART_Right, this.ActualWidth);
            Canvas.SetLeft(this.PART_NextItem, 3*this.ActualWidth/5);
            Canvas.SetLeft(this.PART_PreviousItem, this.ActualWidth / 5);
            Canvas.SetLeft(this.PART_First, 0);
            Canvas.SetLeft(this.PART_Last, 4*this.ActualWidth / 5);
            Canvas.SetLeft(this.PART_CurrentItem, 2 * this.ActualWidth / 5);

            this.PART_Root.RenderTransform = new TranslateTransform();

            var z3 = this.GetItemAt(selectedIndex + 4);
            var f3 = this.GetItemAt(selectedIndex -4);
            var z4 = this.GetItemAt(selectedIndex + 5);
            var f4 = this.GetItemAt(selectedIndex - 5);
            var z5 = this.GetItemAt(selectedIndex + 6);
            var f5 = this.GetItemAt(selectedIndex - 6);
            var RightItem = this.GetItemAt(selectedIndex + 3);
            var LeftItem = this.GetItemAt(selectedIndex-3);
            var currentItem = this.GetItemAt(selectedIndex);
            var nextItem = this.GetItemAt(selectedIndex + 1);
            var previousItem = this.GetItemAt(selectedIndex - 1);
            var FirstItem=this.GetItemAt(selectedIndex - 2);
            var lastItem= this.GetItemAt(selectedIndex + 2);            
            this.PART_Right.Content = RightItem;           
            this.PART_Left.Content = LeftItem;           
            this.PART_First.Content = FirstItem;           
            this.PART_NextItem.Content = nextItem;
            this.PART_PreviousItem.Content = previousItem;
            this.PART_Last.Content = lastItem;
            this.PART3.Content = z3;
            this.PART_3.Content = f3;
            this.PART4.Content = z4;
            this.PART5.Content = z5;
            this.PART_4.Content = f4;
            this.PART_5.Content = f5;
            this.PART_CurrentItem.Content = currentItem;

        }

        public void RunSlideAnimation(double toValue, double fromValue = 0)
        {
            if (!(this.PART_Root.RenderTransform is TranslateTransform))
            {
                this.PART_Root.RenderTransform = new TranslateTransform();
            }

            var story = AnimationFactory.Instance.GetAnimation(this.PART_Root, toValue, fromValue);
            story.Completed += (s, e) =>
                {
                    this.RefreshViewPort(this.SelectedIndex);
                };
            story.Begin();
        }

        private object GetItemAt(int index)
        {
            if (index < 0 || index >= this.Items.Count)
            {
                return null;
            }

            return this.Items[index];
        }

        private bool EnsureTemplateParts()
        {
            return this.PART_CurrentItem != null &&
                this.PART_NextItem != null &&
                this.PART_PreviousItem != null &&
                this.PART_Root != null;
        }

        private void OnPreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex > 0;
        }

        private void OnPreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectedIndex -= 1;
        }

        private void OnNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex < (this.Items.Count - 1);
        }

        private void OnNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectedIndex += 1;
        }
        #endregion

        #region Commands

        public static RoutedUICommand NextCommand = new RoutedUICommand("Next", "Next", typeof(FlipView));
        public static RoutedUICommand PreviousCommand = new RoutedUICommand("Previous", "Previous", typeof(FlipView));

        #endregion

        #region Override methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART3= this.GetTemplateChild("PART3") as ContentControl;
            this.PART_3 = this.GetTemplateChild("PART_3") as ContentControl;
            this.PART4 = this.GetTemplateChild("PART4") as ContentControl;
            this.PART_4 = this.GetTemplateChild("PART_4") as ContentControl;
            this.PART5 = this.GetTemplateChild("PART5") as ContentControl;
            this.PART_5 = this.GetTemplateChild("PART_5") as ContentControl;
            this.PART_Left = this.GetTemplateChild("PART_Left") as ContentControl;
            this.PART_Right = this.GetTemplateChild("PART_Right") as ContentControl;
            this.PART_First = this.GetTemplateChild("PART_First") as ContentControl;
            this.PART_Last = this.GetTemplateChild("PART_Last") as ContentControl;
            this.PART_PreviousItem = this.GetTemplateChild("PART_PreviousItem") as ContentControl;
            this.PART_NextItem = this.GetTemplateChild("PART_NextItem") as ContentControl;
            this.PART_CurrentItem = this.GetTemplateChild("PART_CurrentItem") as ContentControl;
            this.PART_Root = this.GetTemplateChild("PART_Root") as FrameworkElement;
            this.PART_Container = this.GetTemplateChild("PART_Container") as FrameworkElement;

            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
            this.PART_Root.ManipulationStarting += this.OnRootManipulationStarting;
            this.PART_Root.ManipulationDelta += this.OnRootManipulationDelta;
            this.PART_Root.ManipulationCompleted += this.OnRootManipulationCompleted;
            this.PART_Root.TouchDown += PART_Root_TouchDown;
            this.PART_Root.TouchUp += PART_Root_TouchUp;
        }

        private void PART_Root_TouchUp(object sender, TouchEventArgs e)
        {
            var x1 = myPoint.X;
            var y1 = myPoint.Y;
            if (y1 < this.ActualHeight*2/3 && y1 > this.ActualHeight / 3 && (x1 > this.ActualWidth * 3 / 5 || x1 < this.ActualWidth * 2 / 5))
            {
                Point currentPoint = e.GetTouchPoint(this).Position;
                var x2 = currentPoint.X;
                var y2 = currentPoint.Y;
                if (y1 - y2 < 20 && Math.Abs(x1-x2)<5)
                {
                    if (x1 < this.ActualWidth / 5 && this.SelectedIndex>1)
                    {
                        this.SelectedIndex -= 2;
                        n = 2;
                    }
                    if (x1 < this.ActualWidth*2/5 && this.SelectedIndex > 0 && x1 >this.ActualWidth/ 5)
                    {
                        this.SelectedIndex --;
                        n = 1;
                    }
                    if (x1 < this.ActualWidth * 4 / 5 && this.SelectedIndex < this.Items.Count-1 && x1 > this.ActualWidth *3/ 5)
                    {
                        this.SelectedIndex ++;
                        n = 1;
                    }
                    if (x1 > this.ActualWidth * 4 / 5 && this.SelectedIndex < this.Items.Count-2)
                    {
                        this.SelectedIndex +=2;
                        n = 2;
                    }
                }
                
            }
        }

        private void PART_Root_TouchDown(object sender, TouchEventArgs e)
        {
            myPoint= e.GetTouchPoint(this).Position;

        }
        #endregion

    }
}
