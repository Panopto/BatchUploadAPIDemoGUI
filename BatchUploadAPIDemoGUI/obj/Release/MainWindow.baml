   M S B A M L   `   `   `  ����   BatchUploadAPIDemoGUI&     BatchUploadAPIDemoGUI.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Batch Upload API Demo��$��603.6��$	��550�� � Icon$ Logo-PanoptoGlobe.png�� � 
ResizeMode$ CanMinimize=� � 	Resources  �� MergedDictionaries �� Source �� 
TargetType   ChosenButton  BaseChosenButton �� BasedOn��  �� $ APIDemoResources.xaml?�%%   &	        7  �� " ��8   .�-   .���� ���	#FF259216$��3q�._�� $��0,0,0,0q�.+��� 
����ح�$��Upload Destination��$	��Top=�$	��5,0q�$	��2.5q�.��� .+��� -    ��Name$ Server��$
��Left=�$��
134,10,0,0q�	 �� TextWrapping$
	 Wrap=�$��foo.bar.com��$	��Top=�$	��271��$��25���� $��Server:��$
��Left=�$��	10,10,0,0q�$	��Top=�$��30���� $��
User Name:��$
��Left=�$��30��$��	10,45,0,0q�$	��Top=�$��74��
 J� RenderTransformOrigin$
 0.5,0.5&��� -   $ UserID��$
��Left=�$��
134,45,0,0q�$
	 Wrap=�$	��Foo��$	��Top=�$	��271��$��25���� $��	Password:��$
��Left=�$��30��$��	10,80,0,0q�$	��Top=�$��65��G� -    G�Name$ UserPassword��$
��Left=�$��25��$��
134,80,0,0q�$	��Top=�$	��271�� G� Password$	 Bar���� $��
Folder ID:��$
��Left=�$��30��$��
10,115,0,0q�$	��Top=�$��74���� -   $ FolderID��$
��Left=�$��25��$��134,115,0,0q�$
	 Wrap=�$��foo-bar��$	��Top=�$	��271���� 
����ح�$��Upload Source��$	��Top=�$��	5,185,5,0q�$	��2.5q�.��� .+��� $��Upload Mode:��$
��Left=�$��30��$��	10,10,0,0q�$	��Top=�$��89��	� -    	�Name$
 HTTP��$
��HTTP��$
��Left=�$��21��$��
134,15,0,0q�$	��Top=�$��52��	� -   $	 FTP��$	��FTP��$
��Left=�$��21��$��
191,15,0,0q�$	��Top=�$��44��	� -   $
 Disk��$��Disk and Network��$
��Left=�$��21��$��
240,15,0,0q�$	��Top=�$	��115���� -	    ��Name$ SourceIDLabel��$��
User Name:��$
��Left=�$��30��$��	10,45,0,0q�$	��Top=�$��89��$y�Hidden=��� -
   $ FileSourceID��$
��Left=�$��25��$��
134,45,0,0q�$
	 Wrap=�$	��Bar��$	��Top=�$	��271��{�.  $y�Hidden=��� -   $ SourceKeyLabel��$��	Password:��$
��Left=�$��30��$��	10,80,0,0q�$	��Top=�$��89��$y�Hidden=�G� -   $ FileSourceKey��$
��Left=�$��25��$��
134,80,0,0q�$	��Top=�$	��271��$	 foo��{�.  $y�Hidden=��� -   $ SourceFileLabel��$��File:��$
��Left=�$��30��$��
10,115,0,0q�$	��Top=�$��35��$y�Hidden=��� -   $ 	FileInput��$
��Left=�$��25��$��134,115,0,0q�$
	 Wrap=�$��Somefile.mp4��$	��Top=�$	��271��{�.  $y�Hidden=��� -    ��Name$ Browse��$
��Left=�$��25��$	��Top=�$��74��$��410,115,0,0q�$��Browse��$y�Hidden=��� -    ��Name$ ChosenFileViewer��$
��Left=�$	��122��$��134,150,0,0q�$	��Top=�$	��350��$y�Hidden=�.���� -    ��Name$ Files��$	��Top=�$
��Left=��� -   $ Upload��$��Upload��$
��Left=�$��29��$��200,490,0,0q�$	��Top=�$	��121���� -    ��Name$ 	StatusBar��$��35��$��	0,282,0,0q�$��Bottom=�.@��� -   $ Status��$��Ready to upload��