﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ZuegerAdressbook.Extensions;

namespace ZuegerAdressbook.View.Controls
{
	public delegate void FilterRoutedEventHandler(object sender, FilterEventArgs e);
	public delegate void DirectionRoutedEventHandler(object sender, DirectionEventArgs e);

	public enum DirectionEnum
	{
		Default,
		Up,
		Down,
		Left,
		Right,
	}

	[TemplatePart(Name = PART_FilterBox, Type = typeof(TextBox))]
	[TemplatePart(Name = PART_ClearButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_Header, Type = typeof(TextBlock))]
	public class FilterControl : Control
	{
	    private const string PART_FilterBox = "PART_FilterBox";
		private const string PART_ClearButton = "PART_ClearButton";
		private const string PART_Header = "PART_Header";

		public static readonly RoutedEvent FilterEvent;

		public static readonly RoutedEvent DirectionEvent;

		public static readonly RoutedEvent ClearFilterEvent;

        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(FilterControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(FilterControl), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty TargetControlProperty = DependencyProperty.Register("TargetControl", typeof(ItemsControl), typeof(FilterControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty FilterTextBindingPathProperty = DependencyProperty.Register("FilterTextBindingPath", typeof(string), typeof(FilterControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty FilterOnEnterProperty = DependencyProperty.Register("FilterOnEnter", typeof(bool), typeof(FilterControl), new UIPropertyMetadata(false));
        public static readonly DependencyProperty FilterFiringIntervalProperty = DependencyProperty.Register("FilterFiringInterval", typeof(int), typeof(FilterControl), new UIPropertyMetadata(100, new PropertyChangedCallback(OnFilterFiringIntervalChanged)));

        private Button clearButton = null;
		private TextBox filterBox = null;
		private TextBlock textBlock = null;
		private int tickCount;

		private bool byPassEvent = false;

		private readonly DispatcherTimer timer;

	    static FilterControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterControl), new FrameworkPropertyMetadata(typeof(FilterControl)));

			FilterEvent = EventManager.RegisterRoutedEvent("Filter", RoutingStrategy.Bubble, typeof(FilterRoutedEventHandler), typeof(FilterControl));
			DirectionEvent = EventManager.RegisterRoutedEvent("Direction", RoutingStrategy.Bubble, typeof(DirectionRoutedEventHandler), typeof(FilterControl));
			ClearFilterEvent = EventManager.RegisterRoutedEvent("ClearFilter", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(FilterControl));
		}

		public FilterControl()
		{
			timer = new DispatcherTimer(DispatcherPriority.Normal);
			timer.Interval = new TimeSpan(0, 0, 0, 0, FilterFiringInterval);
			timer.Tick += new EventHandler(OnDispatcherTimerTick);
		}

	    public string FilterText
		{
			get { return (string)GetValue(FilterTextProperty); }
			set { SetValue(FilterTextProperty, value); }
		}

	    public string Header
	    {
	        get { return (string)GetValue(HeaderProperty); }
	        set { SetValue(HeaderProperty, value); }
	    }

	    public ItemsControl TargetControl
	    {
	        get { return (ItemsControl)GetValue(TargetControlProperty); }
	        set { SetValue(TargetControlProperty, value); }
	    }

	    public string FilterTextBindingPath
	    {
	        get { return (string)GetValue(FilterTextBindingPathProperty); }
	        set { SetValue(FilterTextBindingPathProperty, value); }
	    }

	    public bool FilterOnEnter
	    {
	        get { return (bool)GetValue(FilterOnEnterProperty); }
	        set { SetValue(FilterOnEnterProperty, value); }
	    }

	    public int FilterFiringInterval
	    {
	        get { return (int)GetValue(FilterFiringIntervalProperty); }
	        set { SetValue(FilterFiringIntervalProperty, value); }
	    }

	    public event FilterRoutedEventHandler Filter
		{
			add { AddHandler(FilterEvent, value); }
			remove { RemoveHandler(FilterEvent, value); }
		}

	    public event DirectionRoutedEventHandler Direction
		{
	        add { AddHandler(DirectionEvent, value); }
	        remove { RemoveHandler(DirectionEvent, value); }
		}

	    public event RoutedEventHandler ClearFilter
		{
			add { AddHandler(ClearFilterEvent, value); }
			remove{ RemoveHandler(ClearFilterEvent, value); }
		}

	    private static void OnFilterFiringIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
	        FilterControl filterBox = d as FilterControl;
	        filterBox.timer.Interval = new TimeSpan(0, 0, 0, 0, filterBox.FilterFiringInterval);
	    }

	    public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			AttachToVisualTree();
		}

	    private void ApplyFilterOnTarget()
		{
			if (TargetControl == null || TargetControl.ItemsSource == null)
			{
				return;
			}

			var collectionView = CollectionViewSource.GetDefaultView(TargetControl.ItemsSource);

			if (collectionView == null)
			{
				throw new InvalidOperationException("The TargetConrol should use ICollectionView as ItemSource.");
			}

			if (string.IsNullOrEmpty(this.FilterTextBindingPath))
			{
				throw new InvalidOperationException("FilterTextBindingPath is not set.");
			}

			collectionView.Filter = (m => (m.DynamicAccess<string>(this.FilterTextBindingPath).IndexOf(this.FilterText, StringComparison.InvariantCultureIgnoreCase) > -1));
		}

		private void ClearFilterOnTarget()
		{
			if (TargetControl == null || TargetControl.ItemsSource == null)
			{
				return;
			}

			var collectionView = CollectionViewSource.GetDefaultView(TargetControl.ItemsSource);

			if (collectionView == null)
			{
				throw new InvalidOperationException("The TargetConrol should use ICollectionView as ItemSource.");
			}

			collectionView.Filter = null;
		}

		private void RaiseFilterEvent()
		{
			var args = new FilterEventArgs(FilterEvent, this, this.FilterText);

			RaiseEvent(args);

			if (!args.IsFilterApplied)
			{
				ApplyFilterOnTarget();
			}
		}

		private void AttachToVisualTree()
		{
			textBlock = GetTemplateChild(PART_Header) as TextBlock;

			filterBox = GetTemplateChild(PART_FilterBox) as TextBox;

			if (filterBox != null)
			{
				filterBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnLostKeyboardFocus);
				filterBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus);
				filterBox.TextChanged += new TextChangedEventHandler(OnFilterBoxTextChanged);
			}

			clearButton = GetTemplateChild(PART_ClearButton) as Button;

			if (clearButton != null)
			{
				clearButton.Click += new RoutedEventHandler(OnClearButtonClick);
			}

			this.KeyDown += new KeyEventHandler(OnControlKeyDown);
			this.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnKeyboardGotFocus);
		}

		private void OnKeyboardGotFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			Keyboard.Focus(filterBox);
		}

		private void OnControlKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && !string.IsNullOrEmpty(FilterText))
			{
				Clear();
			}
			else if (e.Key == Key.Enter && FilterOnEnter)
			{
				RaiseFilterEvent();
			}
			else if (e.Key == Key.Up)
			{
				DirectionEventArgs args = new DirectionEventArgs(DirectionEvent, this, DirectionEnum.Up);
				RaiseEvent(args);
			}
			else if (e.Key == Key.Down)
			{
				DirectionEventArgs args = new DirectionEventArgs(DirectionEvent, this, DirectionEnum.Down);
				RaiseEvent(args);
			}
			else if (e.Key == Key.Right)
			{
				DirectionEventArgs args = new DirectionEventArgs(DirectionEvent, this, DirectionEnum.Right);
				RaiseEvent(args);
			}
			else if (e.Key == Key.Left)
			{
				DirectionEventArgs args = new DirectionEventArgs(DirectionEvent, this, DirectionEnum.Left);
				RaiseEvent(args);
			}
		}

		private void OnClearButtonClick(object sender, RoutedEventArgs e)
		{
			Clear();
		}

		public void Clear()
		{
			byPassEvent = true;
			filterBox.Text = string.Empty;
			Keyboard.Focus(filterBox);

			RoutedEventArgs args = new RoutedEventArgs(ClearFilterEvent, this);
			RaiseEvent(args);

			if (!args.Handled)
			{
				ClearFilterOnTarget();
			}
		}

		private void OnFilterBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!FilterOnEnter)
			{
				tickCount = 0;
				timer.Start();
			}

			if (string.IsNullOrEmpty(filterBox.Text))
			{
				clearButton.Visibility = Visibility.Collapsed;

				if (!filterBox.IsFocused)
				{
					textBlock.Visibility = Visibility.Visible;
				}
				else
				{
					textBlock.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				clearButton.Visibility = Visibility.Visible;
				textBlock.Visibility = Visibility.Collapsed;
			}
		}

		private void OnGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			textBlock.Visibility = Visibility.Collapsed;
		}

		private void OnLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(filterBox.Text))
			{
				textBlock.Visibility = Visibility.Visible;
			}
		}

		private void OnDispatcherTimerTick(object sender, EventArgs e)
		{
			if (tickCount == 2)
			{
				tickCount = 0;
				timer.Stop();

				if (!byPassEvent)
				{
					RaiseFilterEvent();
				}

				byPassEvent = false;
			}

			tickCount++;
		}
	}

	public class FilterEventArgs : RoutedEventArgs
	{
		public string FilterText { get; private set; }

		public bool IsFilterApplied { get; set; }

		public FilterEventArgs()
			: base()
		{
			FilterText = null;
			IsFilterApplied = false;
		}

		public FilterEventArgs(RoutedEvent routedEvent, object source) :
			this(routedEvent, source, string.Empty)
		{
		}

		public FilterEventArgs(RoutedEvent routedEvent, object source, string filterText)
			: base(routedEvent, source)
		{
			FilterText = filterText;
		}
	}

	public class DirectionEventArgs : RoutedEventArgs
	{
		public DirectionEnum Direction
		{
			get;
			private set;
		}

		public DirectionEventArgs()
			: base()
		{
			Direction = DirectionEnum.Default;
		}

		public DirectionEventArgs(RoutedEvent routedEvent, object source) :
			this(routedEvent, source, DirectionEnum.Default)
		{
		}

		public DirectionEventArgs(RoutedEvent routedEvent, object source, DirectionEnum direction)
			: base(routedEvent, source)
		{
			Direction = direction;
		}
	}
}
