﻿#pragma checksum "..\..\EditMenu.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "FA47302E901E23D8504B618993F92E51E1A1EBF68E7956563B9122256D7EEBD9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ProgramEdit;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ProgramEdit {
    
    
    /// <summary>
    /// EditMenu
    /// </summary>
    public partial class EditMenu : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Team;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Player;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Tournament;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Sponsor;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Coach;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\EditMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Section;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ProgramEdit;component/editmenu.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\EditMenu.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Team = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\EditMenu.xaml"
            this.Team.Click += new System.Windows.RoutedEventHandler(this.Team_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Player = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\EditMenu.xaml"
            this.Player.Click += new System.Windows.RoutedEventHandler(this.Player_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Tournament = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\EditMenu.xaml"
            this.Tournament.Click += new System.Windows.RoutedEventHandler(this.Tournament_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Sponsor = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\EditMenu.xaml"
            this.Sponsor.Click += new System.Windows.RoutedEventHandler(this.Sponsor_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Coach = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\EditMenu.xaml"
            this.Coach.Click += new System.Windows.RoutedEventHandler(this.Coach_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Section = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\EditMenu.xaml"
            this.Section.Click += new System.Windows.RoutedEventHandler(this.Section_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

