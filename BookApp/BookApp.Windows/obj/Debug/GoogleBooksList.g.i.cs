﻿

#pragma checksum "C:\Users\Ir Christian Scott\documents\visual studio 2015\Projects\BookApp\BookApp\BookApp.Windows\GoogleBooksList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DC2DDA503356FD7CAFC9F0AD384094CA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookApp
{
    partial class GoogleBooksList : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Page pageRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Data.CollectionViewSource groupedGoogleBooks; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.GridView booksItemView; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ProgressRing dataLoader; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button backButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock googleBooks; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.RadioButton moochBooks; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.SearchBox searchBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button refreshButon; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualStateGroup ResultStates; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState ResultsFound; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState NoResultsFound; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///GoogleBooksList.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            pageRoot = (global::Windows.UI.Xaml.Controls.Page)this.FindName("pageRoot");
            groupedGoogleBooks = (global::Windows.UI.Xaml.Data.CollectionViewSource)this.FindName("groupedGoogleBooks");
            booksItemView = (global::Windows.UI.Xaml.Controls.GridView)this.FindName("booksItemView");
            dataLoader = (global::Windows.UI.Xaml.Controls.ProgressRing)this.FindName("dataLoader");
            backButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("backButton");
            googleBooks = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("googleBooks");
            moochBooks = (global::Windows.UI.Xaml.Controls.RadioButton)this.FindName("moochBooks");
            searchBox = (global::Windows.UI.Xaml.Controls.SearchBox)this.FindName("searchBox");
            refreshButon = (global::Windows.UI.Xaml.Controls.Button)this.FindName("refreshButon");
            ResultStates = (global::Windows.UI.Xaml.VisualStateGroup)this.FindName("ResultStates");
            ResultsFound = (global::Windows.UI.Xaml.VisualState)this.FindName("ResultsFound");
            NoResultsFound = (global::Windows.UI.Xaml.VisualState)this.FindName("NoResultsFound");
        }
    }
}



