   M S B A M L   `   `   `  ����T  PQBR.AnalyzerManagerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:QBR.AnalyzerManagerModule.ViewModels$QBR.AnalyzerManagerModule.ViewModels  5      Q-clr-namespace:QBR.AnalyzerManagerModule.ViewsQBR.AnalyzerManagerModule.Views  5	      M IQBR.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullqKclr-namespace:QBR.Infrastructure.TriggerActions;assembly=QBR.Infrastructure!QBR.Infrastructure.TriggerActions 5
      2    ,QBR.AnalyzerManagerModule.Views.AnalyzerView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      I
viewModels2clr-namespace:QBR.AnalyzerManagerModule.ViewModels     5      c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      n jMicrosoft.Practices.Prism.Interactivity, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \Microsoft.Practices.Prism, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35+prismhttp://www.codeplex.com/prism   5      ;views-clr-namespace:QBR.AnalyzerManagerModule.Views   5	      btriggerActionsKclr-namespace:QBR.Infrastructure.TriggerActions;assembly=QBR.Infrastructure   5
      h	 dMicrosoft.Expression.Interactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bei9http://schemas.microsoft.com/expression/2010/interactions 	 5        � 	Resources  5      <   6QBR.AnalyzerManagerModule.ViewModels.AnalyzerViewModel   DesignViewModel   AnalyzerName   AnalyzerStatus   AnalyzerType��%r   &	          5   
   $ PP123456��5   l   $ AwaitSampleApplication=�5   ~   $	 PRO=�5   g   5      5      .��5!      � .  (System.Windows.Interactivity.Interaction   Triggers 5"   
   Z  TMicrosoft.Practices.Prism.Interactivity.InteractionRequest.InteractionRequestTrigger  5#      3  -System.Windows.Interactivity.EventTriggerBase   SourceObject 6.   �� *ShowTestResultRequest+.  (System.Windows.Interactivity.TriggerBase   Actions. 5$      B 	 <Microsoft.Expression.Interactivity.Core.ChangePropertyAction  8  2System.Windows.Interactivity.TargetedTriggerAction   
TargetName$ 
TestResult��6,      PropertyName$ 
IsExpanded��6D   	   Value$
	 true��6^   >  8QBR.Infrastructure.TriggerActions.ShowNotificationAction  5%      $ 
TestResult��68   5&      /	  )System.Windows.Interactivity.EventTrigger	  5'      
 	  	EventName$
 MouseLeftButtonDown��6   . 5(      6
  0System.Windows.Interactivity.InvokeCommandAction
   
  Command 6(   �� *SelectCommand+6L   5)       � ColumnDefinitions 5,   
   �� 5-      $��25 �6   �� 5.      $��* �6    � RowDefinitions 50   
   �� 51      $��15 �6   �� 52      $��15 �6   �� 53      $��15 �6   �� 54      $
��Auto �6   .+�57   
   �� $��0��6   $��0��6!   $��2��6.   $��4��6B   $��2q�6S    �� CornerRadius$ 5��6g   ��58      �� 59       �� 
TargetType" ��6    �� Triggers 5:      �� 5;       �� Binding 6&   �� *
IsSelected+ �� Value True6E   .a�5<      ��  �� Property � �� �� Value
 � � �6K   �� 5=       � ��
 � d �6P   5>      �� 5?       6&   �� *
IsSelected+	 False6E   .a�5@      ��  � ��
 �����6K   �� 5A       � ��
 �����6O   5B      �� 5C       6&   �� *AnalyzerStatus+ Disconnected6I   .a�5D      ��  � ��
 �����6P   �� 5E       � ��
 �  ��6J   5F      5H      5J      �� 5L   
   $��0��6   $��0��6   $��3��6-   $��Right=�6�   $	��Top=�6�   $��0,5,0,0q�6�   ��6>   �� *AnalyzerType+  AnalyzerTypeToIconConverter �� 	Converter# [ 6�   �� 5N   
   $��0��6   $��1��6!   $��Center=�61   $
��Left=�6L   $
��Bold"�6�   $��5,0,0,0q�6�   ��6g   ��  �� Path$ AnalyzerName���5O      �� 5P      " ��6    5Q      �� 5R       6*   �� *
IsSelected+ True6I   .a�5S   "   ��  � ��
 �����6O   5T      5V      5X      �� 5Y   
   $��0��6   $��1��6    $��Bottom=�60   $��Right=�6K   $��0,0,5,0q�6g    d� 	IsChecked 6x   �� *
IsSelected+6�   �� 5Z   
   -   5        $��AnalyzerState��5Z   �   $��1��6   $��1��6!   $��Center=�61   $
��Left=�6L   $��5,0,0,0q�6�   ��6g   �� $ AnalyzerStatus���5[      �� 5\      " ��6    5]      �� 5^       6(   ��  �� ElementName$ AnalyzerFailure��$ 
Visibility� Visible6i   .a�5_      ��  � y�$ Hidden=�6=   6N   5`      �� 5a       6(   �� *
IsSelected+ True6G   .a�5b   "   ��  � ��
 �����6O   5c      5e      5g      �� 5i   
   -   5        $��AnalyzerFailure��5i   �   $��1��6   $��1��6!   $��Center=�61   $
��Left=�6L   $��5,0,0,0q�6�   
���  ����6g   �� $ AnalyzerFailure���5j      �� 5k      " ��6    5l      �� 5m       6(   �� *AnalyzerFailure+ 	NoFailure6L   .a�5n      ��  � y�$ Hidden=�6=   6N   5o      5q      5s      �� 5u   
   $��2��6   $��1��6"   $��Center=�62   $
��Left=�6M   $��5,0,0,0q�6�   ��6h   �� $ 
VialCaseID� �� StringFormat$ Vial Case: {0}����5v      �� 5w      " ��6    5x      �� 5y       6*   �� *
IsSelected+ True6I   .a�5z   "   ��  � ��
 �����6O   5{      5}      5      9� 5�   
   -   5         9�Name$ 
TestResult��5�   X   $��3��6   $��2��6!   $��0��65    9� 
IsExpanded .    ResultExpanderTemplate#��[ 6l   $��5,0,5,3q�6�   $��Results��6�   .��5�      5   /QBR.AnalyzerManagerModule.Views.TestResultPopup  5�      5�      5�      