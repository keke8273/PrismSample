   M S B A M L   `   `   `  ����W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� 
TargetType   ResultExpanderTemplate � 	Resources  Key �� Color g� 	BeginTime g� Duration �� 
TargetName �� TargetProperty O� KeyTime O� Value	 �� KeyTime
 �� Value R� KeyTime R� Value � RowDefinitions �� CornerRadius � ColumnDefinitions d� 	IsChecked �� Path �� RelativeSource'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� ResourceKey F�Name � LayoutTransform �� ScaleX �� ScaleY �� Triggers P� Property P� Value N� EnterActions  	Timeline1 N� ExitActions  	Timeline2�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml      5      % 
  &	        0�� 5C   2   $ MouseOverStoryboard��6A   10�� 5I   2   $ CheckStoryboard��6A   17  7  �� 5      "  9�6    5   
   ���� 5      HighlightBrush $ 
DodgerBlue��65   6J   �� 5      	Timeline1 .��5      U� $ 00:00:00X�60   $ 
00:00:0.15D�6E   $ Content��6[   $] W(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�6{   .\�5      �� $ 
00:00:0.15��6+   $ 1Q�6@   5	      5
      �� 5      	Timeline2 .��5      U� $ 00:00:00X�60   $ 00:00:00.25D�6E   $ Content��6\   $] W(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�6|   .\�5      �� $ 
00:00:0.25��6+   $ 0Q�6@   5      5      �� 5      MouseOverStoryboard .��5      �� $ Triangle��60   $ (Path.Stroke).Color�6Q   $ 0:0:0.1D�6�   .g�5      �� $	 0:0:0.1��6*   $
 
DodgerBlue��5   2   5      5      �� 5      CheckStoryboard .��5      T� $ (Path.Data)�60   $ Triangle��6X   ..�5      i� $ 0��6-    5      �5      M 1 3 L 6 8 L 11 3�5!      5#      5$      5.      5/      .R�50   
   �  51      �� 52      $
��Auto �6    �� 53      $��* �6    .+�55      �� $��HeaderBorder��6   $��0��6)   |�. $��0,0,0,1q�6G   
�������$��0q�6w   $ 2,2,0,0��6�   ._�56      � $��Center=�6    57      �� 58      $��* �6+   �� 59      $��15 �6+   .+�5;      �� $��Header��6    $��0��6/   #��z��6?   $��5,0,0,0q�6_   $��Center=�6p   
�������6�   d� 5<      $��1��6#    63   �� $ 
IsExpanded� �� *TemplatedParent+��5=      �� 5>      "  d�6.   .R�5?   "   �� $��ToggleBoarder��6)   $��12��6>   $��12��6I   
������  5@   &     5A   *   CommonStates��. 5B   .     	MouseOver��. 5C   2   2  5D   /     5E   .   
Normal��6L   5F   +     5G   *   CheckStates��. 5H   .     Checked��. 5I   2   2 5J   /     5K   .   	Unchecked��6O   5L   +   ._�5N   &   � .+�5O   *   F� $ Triangle��6/   
������ "����@B ��j р�[ ��- ��ا ��j 	
�������$��2��6}   5P   '   5Q   #   5R      5T      5U      5V      �� 5W      $��Content��6   $��1��6.   $y�Visible=�6;    5X      V� 5Y      .~�5Z      �� $ 1Q�6)   $ 0Q�64   5[      5]      5^       5a   
   P� 5b       �@9�
IsExpanded .  5c      �� 5d      8�� 6&    5f      �� 5g      8�� 6&   5i      5k      5l      