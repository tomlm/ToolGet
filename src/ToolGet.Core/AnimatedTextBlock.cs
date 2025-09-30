using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Spinner;

namespace ToolGet.Core
{
    public enum TextAnimationType
    {
        Lines,
        Boxes,
        QuarterBalls,
        HalfBalls,
        Balloons,
        Arcs,
        Dots,
        DotDotDot,
        VerticalBar,
        HorizontalBar,
        SpinArrows,
        Triangles,
        BouncingBalls,
        Wave,
        Braille,
        Sparkle,
        RightArrows,
        LeftArrows,
        DualArrows,
        Staves,
        Pulse,
    }

    public class AnimatedTextBlock : TextBlock
    {
        public static readonly StyledProperty<TextAnimationType> AnimationTypeProperty =
            AvaloniaProperty.Register<AnimatedTextBlock, TextAnimationType>(nameof(AnimationType));

        public static readonly StyledProperty<IEnumerable<string>?> FramesProperty =
            AvaloniaProperty.Register<AnimatedTextBlock, IEnumerable<string>?>(nameof(Frames));

        public static readonly StyledProperty<bool> IsAnimatingProperty =
            AvaloniaProperty.Register<AnimatedTextBlock, bool>(nameof(IsAnimating), defaultValue: false);

        public static readonly StyledProperty<int> FrameIntervalProperty =
            AvaloniaProperty.Register<AnimatedTextBlock, int>(nameof(FrameInterval), 100);

        private CancellationTokenSource? _cts;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == IsAnimatingProperty)
            {
                OnIsAnimatingChanged(this, (AvaloniaPropertyChangedEventArgs<bool>)change);
            }
        }


        public TextAnimationType AnimationType
        {
            get => GetValue(AnimationTypeProperty);
            set
            {
                SetValue(AnimationTypeProperty, value);
                Frames = value switch
                {
                    TextAnimationType.Lines => Animations.Lines,
                    TextAnimationType.Boxes => Animations.Boxes,
                    TextAnimationType.QuarterBalls => Animations.QuarterBalls,
                    TextAnimationType.HalfBalls => Animations.HalfBalls,
                    TextAnimationType.Balloons => Animations.Balloons,
                    TextAnimationType.Arcs => Animations.Arcs,
                    TextAnimationType.Dots => Animations.Dots,
                    TextAnimationType.DotDotDot => Animations.DotDotDot,
                    TextAnimationType.VerticalBar => Animations.VerticalBar,
                    TextAnimationType.HorizontalBar => Animations.HorizontalBar,
                    TextAnimationType.SpinArrows => Animations.SpinArrows,
                    TextAnimationType.Triangles => Animations.Triangles,
                    TextAnimationType.BouncingBalls => Animations.BouncingBalls,
                    TextAnimationType.Wave => Animations.Wave,
                    TextAnimationType.Braille => Animations.Braille,
                    TextAnimationType.Sparkle => Animations.Sparkle,
                    TextAnimationType.RightArrows => Animations.RightArrows,
                    TextAnimationType.LeftArrows => Animations.LeftArrows,
                    TextAnimationType.DualArrows => Animations.DualArrows,
                    TextAnimationType.Staves => Animations.Staves,
                    TextAnimationType.Pulse => Animations.Pulse,
                    _ => Array.Empty<string>(),
                };
            }
        }

        public IEnumerable<string>? Frames
        {
            get => GetValue(FramesProperty);
            set => SetValue(FramesProperty, value);
        }

        public bool IsAnimating
        {
            get => GetValue(IsAnimatingProperty);
            set => SetValue(IsAnimatingProperty, value);
        }

        /// <summary>
        /// How long to show each full frame (milliseconds). Default 2000ms.
        /// </summary>
        public int FrameInterval
        {
            get => GetValue(FrameIntervalProperty);
            set => SetValue(FrameIntervalProperty, value);
        }

        private static void OnIsAnimatingChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (sender is AnimatedTextBlock ctl)
            {
                if (e.NewValue.GetValueOrDefault())
                    ctl.StartAnimation();
                else
                    ctl.StopAnimation();
            }
        }

        private void StartAnimation()
        {
            // Cancel any existing animation and start a new one
            StopAnimation();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            _ = RunLoopAsync(token);
        }

        private void StopAnimation()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                _cts.Cancel();

            _cts?.Dispose();
            _cts = null;
        }

        private async Task RunLoopAsync(CancellationToken token)
        {
            var frames = Frames?.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (frames == null || frames.Length == 0)
            {
                // nothing to animate
                return;
            }

            var frameInterval = Math.Max(0, FrameInterval);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var frame in frames)
                    {
                        if (token.IsCancellationRequested) break;

                        // Reveal characters one by one
                        if (frame.Length == 0)
                        {
                            await UpdateTextAsync(string.Empty, token);
                        }
                        else
                        {
                            await UpdateTextAsync(frame, token);
                        }

                        await Task.Delay(frameInterval, token).ContinueWith(_ => { }, TaskScheduler.Default);
                    }
                }
            }
            catch (OperationCanceledException) { /* expected on stop */ }
            finally
            {
                // Optionally clear text when stopped:
                // await UpdateTextAsync(string.Empty, CancellationToken.None);
            }
        }

        private Task UpdateTextAsync(string text, CancellationToken token)
        {
            // Ensure we update on UI thread
            if (Dispatcher.UIThread.CheckAccess())
            {
                Text = text;
                return Task.CompletedTask;
            }

            return Dispatcher.UIThread.InvokeAsync(() => Text = text, DispatcherPriority.Background, token).GetTask();
        }
    }
}