   M S B A M L   `   `   `  ����M  IQBR.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulleEclr-namespace:QBR.Infrastructure.Commands;assembly=QBR.InfrastructureQBR.Infrastructure.Commands  5   	   gFclr-namespace:QBR.Infrastructure.Constants;assembly=QBR.InfrastructureQBR.Infrastructure.Constants  5   	   qKclr-namespace:QBR.Infrastructure.TriggerActions;assembly=QBR.Infrastructure!QBR.Infrastructure.TriggerActions  5	   	    	QBR.Shell   QBR.Shell.Views.Shell   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ` \Microsoft.Practices.Prism, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e352regions$http://www.codeplex.com/CompositeWPF  5   	   TcommandsEclr-namespace:QBR.Infrastructure.Commands;assembly=QBR.Infrastructure  5   	   V	constantsFclr-namespace:QBR.Infrastructure.Constants;assembly=QBR.Infrastructure  5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   n jMicrosoft.Practices.Prism.Interactivity, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e358interactionRequesthttp://www.codeplex.com/prism   5   	   btriggerActionsKclr-namespace:QBR.Infrastructure.TriggerActions;assembly=QBR.Infrastructure   5	   	   $	��800��5
   5   $	��620��6B     � 
ResizeMode$  NoResize=�5   ;    � Title 5
   	   �� *ApplicationNameAndVersion+(   "QBR.Infrastructure.Commands.Closed   Command 5   	   �� *ClosedCommand+.��5      � .  (System.Windows.Interactivity.Interaction   Triggers 5   
   Z  TMicrosoft.Practices.Prism.Interactivity.InteractionRequest.InteractionRequestTrigger  5      3  -System.Windows.Interactivity.EventTriggerBase   SourceObject 6;   �� *ErrorNotificationRequest+.  (System.Windows.Interactivity.TriggerBase   Actions. 5      9   3QBR.Infrastructure.TriggerActions.PopupWindowAction     IsModal . !   CenterOverAssociatedObject . 6B   5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   .   (QBR.Infrastructure.Constants.RegionNames	   UserDataRegion5  /Microsoft.Practices.Prism.Regions.RegionManager
   
RegionName#
 Z	 6&   6{   �� 5   
   $��1��6      TestProgressRegion#
 Z 6&   6   �� 5   
   $��2��6      AnalyzerRegion#
 Z 6&   6{   5      5      