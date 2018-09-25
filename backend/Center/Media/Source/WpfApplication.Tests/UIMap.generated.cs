namespace Gorba.Center.Media.WpfApplication.Tests
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\AdornerThumb.xaml */
    public partial class UIAdornerThumb : WpfCustom /* WPF Class = UserControl */
    {

        public UIAdornerThumb(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AdornerThumb";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\AudioSkimmingElement.xaml */
    public partial class UIAudioSkimmingElement : WpfCustom /* WPF Class = UserControl */
    {

        public UIAudioSkimmingElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioSkimmingElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\CycleDetailsNavigator.xaml */
    public partial class UICycleDetailsNavigator : WpfCustom /* WPF Class = UserControl */
    {

        public UICycleDetailsNavigator(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.CycleDetailsNavigator";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\CycleNavigation.xaml */
    public partial class UICycleNavigation : WpfCustom /* WPF Class = UserControl */
    {

        public UICycleNavigation(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.CycleNavigation";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\FormulaManager.xaml */
    public partial class UIFormulaManager : WpfCustom /* WPF Class = UserControl */
    {

        public UIFormulaManager(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.FormulaManager";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElementDetails.xaml */
    public partial class UILayoutElementDetails : WpfCustom /* WPF Class = UserControl */
    {

        public UILayoutElementDetails(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayoutElementDetails";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutNavigation.xaml */
    public partial class UILayoutNavigation : WpfCustom /* WPF Class = UserControl */
    {

        public UILayoutNavigation(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayoutNavigation";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutPreviewRenderer.xaml */
    public partial class UILayoutPreviewRenderer : WpfCustom /* WPF Class = UserControl */
    {

        public UILayoutPreviewRenderer(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayoutPreviewRenderer";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LedPreviewRenderer.xaml */
    public partial class UILedPreviewRenderer : WpfCustom /* WPF Class = UserControl */
    {

        public UILedPreviewRenderer(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedPreviewRenderer";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MediaShellWindow.xaml */
    public partial class UIMediaShellWindow : WpfWindow /* WPF Class = TrackingWindow */
    {
        private WpfButton fileMenu;
        private WpfButton editMenu;
        private WpfButton viewMenu;
        private WpfCustom sidebarColumn;
        private UIAudioEditor audioEditor;
        private UILayoutEditor layoutEditor;
        private UILedEditor ledEditor;
        private UITftEditorToolbar tftEditorToolbar;
        private UIAudioEditorToolbar audioEditorToolbar;
        private UILedEditorToolbar ledEditorToolbar;
        private UILayoutNavigation layoutNavigation;
        private UIResolutionNavigator resolutionNavigator;
        private UIZoomIndicator zoomIndicator;
        private UILayoutNavigationDialog layoutNavigationDialog;
        private UIResolutionNavigationDialog resolutionNavigationDialog;
        private UILayoutElementDetails layoutElementDetails;
        private UILayerEditor layerEditor;
        private UICycleDetailsNavigator cycleDetailsNavigator;
        private WpfCustom cycleNavigationGridSplitter;
        private UIMainMenuDialog mainMenuDialog;
        private UIEditMenuDialog editMenuDialog;
        private UIViewMenuDialog viewMenuDialog;
        private UIConnectionExceptionDialog connectionExceptionDialog;
        private UIChangePasswordDialog changePasswordDialog;
        private UICheckInDialog checkInDialog;
        private WpfStatusBar statusBar;

        public UIMediaShellWindow(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.MediaShellWindow";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfButton FileMenu
        {
            get
            {
                if (this.fileMenu == null)
                {
                    this.fileMenu = new WpfButton(this);
                    this.fileMenu.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_FileMenu";
                }

                return this.fileMenu;
            }
        }

        public WpfButton EditMenu
        {
            get
            {
                if (this.editMenu == null)
                {
                    this.editMenu = new WpfButton(this);
                    this.editMenu.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_EditMenu";
                }

                return this.editMenu;
            }
        }

        public WpfButton ViewMenu
        {
            get
            {
                if (this.viewMenu == null)
                {
                    this.viewMenu = new WpfButton(this);
                    this.viewMenu.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_ViewMenu";
                }

                return this.viewMenu;
            }
        }

        public WpfCustom SidebarColumn
        {
            get
            {
                if (this.sidebarColumn == null)
                {
                    this.sidebarColumn = new WpfCustom(this);
                    this.sidebarColumn.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_SidebarColumn";
                }

                return this.sidebarColumn;
            }
        }

        public UIAudioEditor AudioEditor
        {
            get
            {
                if (this.audioEditor == null)
                {
                    this.audioEditor = new UIAudioEditor(this, "MediaShellWindow_AudioEditor");
                }

                return this.audioEditor;
            }
        }

        public UILayoutEditor LayoutEditor
        {
            get
            {
                if (this.layoutEditor == null)
                {
                    this.layoutEditor = new UILayoutEditor(this, "MediaShellWindow_LayoutEditor");
                }

                return this.layoutEditor;
            }
        }

        public UILedEditor LedEditor
        {
            get
            {
                if (this.ledEditor == null)
                {
                    this.ledEditor = new UILedEditor(this, "MediaShellWindow_LedEditor");
                }

                return this.ledEditor;
            }
        }

        public UITftEditorToolbar TftEditorToolbar
        {
            get
            {
                if (this.tftEditorToolbar == null)
                {
                    this.tftEditorToolbar = new UITftEditorToolbar(this, "MediaShellWindow_TftEditorToolbar");
                }

                return this.tftEditorToolbar;
            }
        }

        public UIAudioEditorToolbar AudioEditorToolbar
        {
            get
            {
                if (this.audioEditorToolbar == null)
                {
                    this.audioEditorToolbar = new UIAudioEditorToolbar(this, "MediaShellWindow_AudioEditorToolbar");
                }

                return this.audioEditorToolbar;
            }
        }

        public UILedEditorToolbar LedEditorToolbar
        {
            get
            {
                if (this.ledEditorToolbar == null)
                {
                    this.ledEditorToolbar = new UILedEditorToolbar(this, "MediaShellWindow_LedEditorToolbar");
                }

                return this.ledEditorToolbar;
            }
        }

        public UILayoutNavigation LayoutNavigation
        {
            get
            {
                if (this.layoutNavigation == null)
                {
                    this.layoutNavigation = new UILayoutNavigation(this, "MediaShellWindow_LayoutNavigation");
                }

                return this.layoutNavigation;
            }
        }

        public UIResolutionNavigator ResolutionNavigator
        {
            get
            {
                if (this.resolutionNavigator == null)
                {
                    this.resolutionNavigator = new UIResolutionNavigator(this, "MediaShellWindow_ResolutionNavigator");
                }

                return this.resolutionNavigator;
            }
        }

        public UIZoomIndicator ZoomIndicator
        {
            get
            {
                if (this.zoomIndicator == null)
                {
                    this.zoomIndicator = new UIZoomIndicator(this, "MediaShellWindow_ZoomIndicator");
                }

                return this.zoomIndicator;
            }
        }

        public UILayoutNavigationDialog LayoutNavigationDialog
        {
            get
            {
                if (this.layoutNavigationDialog == null)
                {
                    this.layoutNavigationDialog = new UILayoutNavigationDialog(this, "MediaShellWindow_LayoutNavigationDialog");
                }

                return this.layoutNavigationDialog;
            }
        }

        public UIResolutionNavigationDialog ResolutionNavigationDialog
        {
            get
            {
                if (this.resolutionNavigationDialog == null)
                {
                    this.resolutionNavigationDialog = new UIResolutionNavigationDialog(this, "MediaShellWindow_ResolutionNavigationDialog");
                }

                return this.resolutionNavigationDialog;
            }
        }

        public UILayoutElementDetails LayoutElementDetails
        {
            get
            {
                if (this.layoutElementDetails == null)
                {
                    this.layoutElementDetails = new UILayoutElementDetails(this, "MediaShellWindow_LayoutElementDetails");
                }

                return this.layoutElementDetails;
            }
        }

        public UILayerEditor LayerEditor
        {
            get
            {
                if (this.layerEditor == null)
                {
                    this.layerEditor = new UILayerEditor(this, "MediaShellWindow_LayerEditor");
                }

                return this.layerEditor;
            }
        }

        public UICycleDetailsNavigator CycleDetailsNavigator
        {
            get
            {
                if (this.cycleDetailsNavigator == null)
                {
                    this.cycleDetailsNavigator = new UICycleDetailsNavigator(this, "MediaShellWindow_CycleDetailsNavigator");
                }

                return this.cycleDetailsNavigator;
            }
        }

        public WpfCustom CycleNavigationGridSplitter
        {
            get
            {
                if (this.cycleNavigationGridSplitter == null)
                {
                    this.cycleNavigationGridSplitter = new WpfCustom(this);
                    this.cycleNavigationGridSplitter.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_CycleNavigationGridSplitter";
                }

                return this.cycleNavigationGridSplitter;
            }
        }

        public UIMainMenuDialog MainMenuDialog
        {
            get
            {
                if (this.mainMenuDialog == null)
                {
                    this.mainMenuDialog = new UIMainMenuDialog(this, "MediaShellWindow_MainMenuDialog");
                }

                return this.mainMenuDialog;
            }
        }

        public UIEditMenuDialog EditMenuDialog
        {
            get
            {
                if (this.editMenuDialog == null)
                {
                    this.editMenuDialog = new UIEditMenuDialog(this, "MediaShellWindow_EditMenuDialog");
                }

                return this.editMenuDialog;
            }
        }

        public UIViewMenuDialog ViewMenuDialog
        {
            get
            {
                if (this.viewMenuDialog == null)
                {
                    this.viewMenuDialog = new UIViewMenuDialog(this, "MediaShellWindow_ViewMenuDialog");
                }

                return this.viewMenuDialog;
            }
        }

        public UIConnectionExceptionDialog ConnectionExceptionDialog
        {
            get
            {
                if (this.connectionExceptionDialog == null)
                {
                    this.connectionExceptionDialog = new UIConnectionExceptionDialog(this, "MediaShellWindow_ConnectionExceptionDialog");
                }

                return this.connectionExceptionDialog;
            }
        }

        public UIChangePasswordDialog ChangePasswordDialog
        {
            get
            {
                if (this.changePasswordDialog == null)
                {
                    this.changePasswordDialog = new UIChangePasswordDialog(this, "MediaShellWindow_ChangePasswordDialog");
                }

                return this.changePasswordDialog;
            }
        }

        public UICheckInDialog CheckInDialog
        {
            get
            {
                if (this.checkInDialog == null)
                {
                    this.checkInDialog = new UICheckInDialog(this, "MediaShellWindow_CheckInDialog");
                }

                return this.checkInDialog;
            }
        }

        public WpfStatusBar StatusBar
        {
            get
            {
                if (this.statusBar == null)
                {
                    this.statusBar = new WpfStatusBar(this);
                    this.statusBar.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow_StatusBar";
                }

                return this.statusBar;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\PopupWindow.xaml */
    public partial class UIPopupWindow : WpfCustom /* WPF Class = UserControl */
    {

        public UIPopupWindow(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.PopupWindow";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\QuickEditAdorner.xaml */
    public partial class UIQuickEditAdorner : WpfButton /* WPF Class = Button */
    {

        public UIQuickEditAdorner(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.QuickEditAdorner";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\ResolutionNavigator.xaml */
    public partial class UIResolutionNavigator : WpfCustom /* WPF Class = UserControl */
    {

        public UIResolutionNavigator(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ResolutionNavigator";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\ReusableEntitySelector.xaml */
    public partial class UIReusableEntitySelector : WpfCustom /* WPF Class = UserControl */
    {

        public UIReusableEntitySelector(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ReusableEntitySelector";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\SkimmingElement.xaml */
    public partial class UISkimmingElement : WpfCustom /* WPF Class = UserControl */
    {

        public UISkimmingElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SkimmingElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\AnimationEditor.xaml */
    public partial class UIAnimationEditor : WpfCustom /* WPF Class = UserControl */
    {

        public UIAnimationEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AnimationEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\DictionarySelector.xaml */
    public partial class UIDictionarySelector : WpfCustom /* WPF Class = UserControl */
    {

        public UIDictionarySelector(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.DictionarySelector";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\EditableTextblock.xaml */
    public partial class UIEditableTextblock : WpfCustom /* WPF Class = UserControl */
    {

        public UIEditableTextblock(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.EditableTextblock";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\FontPreview.xaml */
    public partial class UIFontPreview : WpfCustom /* WPF Class = UserControl */
    {

        public UIFontPreview(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.FontPreview";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\FormulaEditor.xaml */
    public partial class UIFormulaEditor : WpfCustom /* WPF Class = UserControl */
    {

        public UIFormulaEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.FormulaEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\LedGridDisplay.xaml */
    public partial class UILedGridDisplay : WpfCustom /* WPF Class = UserControl */
    {

        public UILedGridDisplay(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedGridDisplay";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\NewEntityButton.xaml */
    public partial class UINewEntityButton : WpfCustom /* WPF Class = UserControl */
    {

        public UINewEntityButton(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.NewEntityButton";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\PlaybackControl.xaml */
    public partial class UIPlaybackControl : WpfCustom /* WPF Class = UserControl */
    {

        public UIPlaybackControl(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.PlaybackControl";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\ReusableList.xaml */
    public partial class UIReusableList : WpfCustom /* WPF Class = UserControl */
    {

        public UIReusableList(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ReusableList";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\SelectableImage.xaml */
    public partial class UISelectableImage : WpfButton /* WPF Class = Button */
    {

        public UISelectableImage(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SelectableImage";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\TextReplaceControl.xaml */
    public partial class UITextReplaceControl : WpfCustom /* WPF Class = UserControl */
    {

        public UITextReplaceControl(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.TextReplaceControl";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\VirtualDisplaySelector.xaml */
    public partial class UIVirtualDisplaySelector : WpfCustom /* WPF Class = UserControl */
    {

        public UIVirtualDisplaySelector(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.VirtualDisplaySelector";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Controls\ZoomIndicator.xaml */
    public partial class UIZoomIndicator : WpfCustom /* WPF Class = UserControl */
    {
        private WpfButton zoomIn;

        public UIZoomIndicator(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ZoomIndicator";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfButton ZoomIn
        {
            get
            {
                if (this.zoomIn == null)
                {
                    this.zoomIn = new WpfButton(this);
                    this.zoomIn.SearchProperties[WpfControl.PropertyNames.AutomationId] = "ZoomIndicator_ZoomIn";
                }

                return this.zoomIn;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Editors\AudioEditor.xaml */
    public partial class UIAudioEditor : WpfCustom /* WPF Class = EditorViewBase */
    {
        private UIDictionarySelectorDialog dictionarySelectorDialog;

        public UIAudioEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public UIDictionarySelectorDialog DictionarySelectorDialog
        {
            get
            {
                if (this.dictionarySelectorDialog == null)
                {
                    this.dictionarySelectorDialog = new UIDictionarySelectorDialog(this, "AudioEditor_DictionarySelectorDialog");
                }

                return this.dictionarySelectorDialog;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Editors\CsvEditor.xaml */
    public partial class UICsvEditor : WpfWindow /* WPF Class = TrackingWindow */
    {

        public UICsvEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.CsvEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Editors\LayerEditor.xaml */
    public partial class UILayerEditor : WpfCustom /* WPF Class = EditorViewBase */
    {

        public UILayerEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayerEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Editors\LayoutEditor.xaml */
    public partial class UILayoutEditor : WpfCustom /* WPF Class = GraphicalEditorViewBase */
    {
        private UILayoutPreviewRenderer layoutPreviewRenderer;
        private UIEditDynamicTextPopup editDynamicTextPopup;

        public UILayoutEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayoutEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public UILayoutPreviewRenderer LayoutPreviewRenderer
        {
            get
            {
                if (this.layoutPreviewRenderer == null)
                {
                    this.layoutPreviewRenderer = new UILayoutPreviewRenderer(this, "LayoutEditor_LayoutPreviewRenderer");
                }

                return this.layoutPreviewRenderer;
            }
        }

        public UIEditDynamicTextPopup EditDynamicTextPopup
        {
            get
            {
                if (this.editDynamicTextPopup == null)
                {
                    this.editDynamicTextPopup = new UIEditDynamicTextPopup(this, "LayoutEditor_EditDynamicTextPopup");
                }

                return this.editDynamicTextPopup;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Editors\LedEditor.xaml */
    public partial class UILedEditor : WpfCustom /* WPF Class = GraphicalEditorViewBase */
    {
        private UILedPreviewRenderer ledPreviewRenderer;
        private UIEditDynamicTextPopup editDynamicTextPopup;

        public UILedEditor(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedEditor";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public UILedPreviewRenderer LedPreviewRenderer
        {
            get
            {
                if (this.ledPreviewRenderer == null)
                {
                    this.ledPreviewRenderer = new UILedPreviewRenderer(this, "LedEditor_LedPreviewRenderer");
                }

                return this.ledPreviewRenderer;
            }
        }

        public UIEditDynamicTextPopup EditDynamicTextPopup
        {
            get
            {
                if (this.editDynamicTextPopup == null)
                {
                    this.editDynamicTextPopup = new UIEditDynamicTextPopup(this, "LedEditor_EditDynamicTextPopup");
                }

                return this.editDynamicTextPopup;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\AnimationEditorDialog.xaml */
    public partial class UIAnimationEditorDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIAnimationEditorDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AnimationEditorDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\ConsistencyCheckDialog.xaml */
    public partial class UIConsistencyCheckDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIConsistencyCheckDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ConsistencyCheckDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\CreatePhysicalScreenPopup.xaml */
    public partial class UICreatePhysicalScreenPopup : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UICreatePhysicalScreenPopup(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.CreatePhysicalScreenPopup";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\DictionarySelectorDialog.xaml */
    public partial class UIDictionarySelectorDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIDictionarySelectorDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.DictionarySelectorDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\EditDynamicTextPopup.xaml */
    public partial class UIEditDynamicTextPopup : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIEditDynamicTextPopup(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.EditDynamicTextPopup";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\EditMenuDialog.xaml */
    public partial class UIEditMenuDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIEditMenuDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.EditMenuDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\FormulaEditorDialog.xaml */
    public partial class UIFormulaEditorDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIFormulaEditorDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.FormulaEditorDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\LayoutNavigationDialog.xaml */
    public partial class UILayoutNavigationDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UILayoutNavigationDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LayoutNavigationDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\MainMenuDialog.xaml */
    public partial class UIMainMenuDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {
        private WpfCustom new;
        private WpfCustom open;
        private WpfCustom save;
        private WpfCustom checkin;
        private WpfCustom saveAs;
        private UISaveAsScreen saveAsScreen;
        private WpfCustom import;
        private WpfCustom export;
        private WpfCustom resourceManagement;
        private WpfCustom textualReplacement;
        private WpfCustom formulaManager;
        private WpfCustom options;
        private WpfCustom about;
        private WpfCustom exit;

        public UIMainMenuDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.MainMenuDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfCustom New
        {
            get
            {
                if (this.new == null)
                {
                    this.new = new WpfCustom(this);
                    this.new.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_New";
                }

                return this.new;
            }
        }

        public WpfCustom Open
        {
            get
            {
                if (this.open == null)
                {
                    this.open = new WpfCustom(this);
                    this.open.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Open";
                }

                return this.open;
            }
        }

        public WpfCustom Save
        {
            get
            {
                if (this.save == null)
                {
                    this.save = new WpfCustom(this);
                    this.save.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Save";
                }

                return this.save;
            }
        }

        public WpfCustom Checkin
        {
            get
            {
                if (this.checkin == null)
                {
                    this.checkin = new WpfCustom(this);
                    this.checkin.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Checkin";
                }

                return this.checkin;
            }
        }

        public WpfCustom SaveAs
        {
            get
            {
                if (this.saveAs == null)
                {
                    this.saveAs = new WpfCustom(this);
                    this.saveAs.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_SaveAs";
                }

                return this.saveAs;
            }
        }

        public UISaveAsScreen SaveAsScreen
        {
            get
            {
                if (this.saveAsScreen == null)
                {
                    this.saveAsScreen = new UISaveAsScreen(this, "MainMenuDialog_SaveAsScreen");
                }

                return this.saveAsScreen;
            }
        }

        public WpfCustom Import
        {
            get
            {
                if (this.import == null)
                {
                    this.import = new WpfCustom(this);
                    this.import.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Import";
                }

                return this.import;
            }
        }

        public WpfCustom Export
        {
            get
            {
                if (this.export == null)
                {
                    this.export = new WpfCustom(this);
                    this.export.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Export";
                }

                return this.export;
            }
        }

        public WpfCustom ResourceManagement
        {
            get
            {
                if (this.resourceManagement == null)
                {
                    this.resourceManagement = new WpfCustom(this);
                    this.resourceManagement.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_ResourceManagement";
                }

                return this.resourceManagement;
            }
        }

        public WpfCustom TextualReplacement
        {
            get
            {
                if (this.textualReplacement == null)
                {
                    this.textualReplacement = new WpfCustom(this);
                    this.textualReplacement.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_TextualReplacement";
                }

                return this.textualReplacement;
            }
        }

        public WpfCustom FormulaManager
        {
            get
            {
                if (this.formulaManager == null)
                {
                    this.formulaManager = new WpfCustom(this);
                    this.formulaManager.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_FormulaManager";
                }

                return this.formulaManager;
            }
        }

        public WpfCustom Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = new WpfCustom(this);
                    this.options.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Options";
                }

                return this.options;
            }
        }

        public WpfCustom About
        {
            get
            {
                if (this.about == null)
                {
                    this.about = new WpfCustom(this);
                    this.about.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_About";
                }

                return this.about;
            }
        }

        public WpfCustom Exit
        {
            get
            {
                if (this.exit == null)
                {
                    this.exit = new WpfCustom(this);
                    this.exit.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MainMenuDialog_Exit";
                }

                return this.exit;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\ResolutionNavigationDialog.xaml */
    public partial class UIResolutionNavigationDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIResolutionNavigationDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ResolutionNavigationDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\SelectCycleTypePopup.xaml */
    public partial class UISelectCycleTypePopup : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UISelectCycleTypePopup(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SelectCycleTypePopup";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\SelectMediaPopup.xaml */
    public partial class UISelectMediaPopup : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UISelectMediaPopup(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SelectMediaPopup";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\SelectSectionTypePopup.xaml */
    public partial class UISelectSectionTypePopup : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UISelectSectionTypePopup(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SelectSectionTypePopup";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\TriggerEditorDialog.xaml */
    public partial class UITriggerEditorDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UITriggerEditorDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.TriggerEditorDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Interaction\ViewMenuDialog.xaml */
    public partial class UIViewMenuDialog : WpfCustom /* WPF Class = InteractionDialogBase */
    {

        public UIViewMenuDialog(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ViewMenuDialog";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AnalogClockLayoutElement.xaml */
    public partial class UIAnalogClockLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIAnalogClockLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AnalogClockLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AudioDynamicTtsLayoutElement.xaml */
    public partial class UIAudioDynamicTtsLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIAudioDynamicTtsLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioDynamicTtsLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AudioFileLayoutElement.xaml */
    public partial class UIAudioFileLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIAudioFileLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioFileLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AudioOutputFrame.xaml */
    public partial class UIAudioOutputFrame : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIAudioOutputFrame(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioOutputFrame";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AudioPauseLayoutElement.xaml */
    public partial class UIAudioPauseLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIAudioPauseLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioPauseLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\AudioStaticTtsLayoutElement.xaml */
    public partial class UIAudioStaticTtsLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {
        private WpfEdit textContainer;

        public UIAudioStaticTtsLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioStaticTtsLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfEdit TextContainer
        {
            get
            {
                if (this.textContainer == null)
                {
                    this.textContainer = new WpfEdit(this);
                    this.textContainer.SearchProperties[WpfControl.PropertyNames.AutomationId] = "StaticTtsLayoutElement_TextContainer";
                }

                return this.textContainer;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\DynamicTextLayoutElement.xaml */
    public partial class UIDynamicTextLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIDynamicTextLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.DynamicTextLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\FrameLayoutElement.xaml */
    public partial class UIFrameLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIFrameLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.FrameLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\ImageListLayoutElement.xaml */
    public partial class UIImageListLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIImageListLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ImageListLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\LedDynamicTextLayoutElement.xaml */
    public partial class UILedDynamicTextLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UILedDynamicTextLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedDynamicTextLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\LedImageLayoutElement.xaml */
    public partial class UILedImageLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UILedImageLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedImageLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\LedRectangleLayoutElement.xaml */
    public partial class UILedRectangleLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UILedRectangleLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedRectangleLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\LedStaticTextLayoutElement.xaml */
    public partial class UILedStaticTextLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UILedStaticTextLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedStaticTextLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\LiveStreamLayoutElement.xaml */
    public partial class UILiveStreamLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UILiveStreamLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LiveStreamLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\PictureLayoutElement.xaml */
    public partial class UIPictureLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIPictureLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.PictureLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\RssTickerElement.xaml */
    public partial class UIRssTickerElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIRssTickerElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.RssTickerElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\StaticTextLayoutElement.xaml */
    public partial class UIStaticTextLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIStaticTextLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.StaticTextLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\LayoutElements\VideoLayoutElement.xaml */
    public partial class UIVideoLayoutElement : WpfCustom /* WPF Class = EditableLayoutElementBase */
    {

        public UIVideoLayoutElement(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.VideoLayoutElement";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\ExportScreen.xaml */
    public partial class UIExportScreen : WpfCustom /* WPF Class = UserControl */
    {

        public UIExportScreen(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ExportScreen";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\ImportScreen.xaml */
    public partial class UIImportScreen : WpfCustom /* WPF Class = UserControl */
    {

        public UIImportScreen(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ImportScreen";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\NewProjectScreen.xaml */
    public partial class UINewProjectScreen : WpfCustom /* WPF Class = UserControl */
    {

        public UINewProjectScreen(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.NewProjectScreen";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\ProjectListScreen.xaml */
    public partial class UIProjectListScreen : WpfCustom /* WPF Class = UserControl */
    {

        public UIProjectListScreen(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ProjectListScreen";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\ResourceManagerView.xaml */
    public partial class UIResourceManagerView : WpfCustom /* WPF Class = UserControl */
    {

        public UIResourceManagerView(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ResourceManagerView";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\MainMenu\SaveAsScreen.xaml */
    public partial class UISaveAsScreen : WpfCustom /* WPF Class = UserControl */
    {

        public UISaveAsScreen(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.SaveAsScreen";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Toolbars\AudioEditorToolbar.xaml */
    public partial class UIAudioEditorToolbar : WpfCustom /* WPF Class = UserControl */
    {

        public UIAudioEditorToolbar(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.AudioEditorToolbar";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Toolbars\LedEditorToolbar.xaml */
    public partial class UILedEditorToolbar : WpfCustom /* WPF Class = UserControl */
    {
        private WpfRadioButton moveTool;
        private WpfRadioButton zoomTool;
        private WpfRadioButton handTool;
        private WpfRadioButton dynamicTextTool;
        private WpfRadioButton staticTextTool;
        private WpfRadioButton imageTool;
        private WpfRadioButton ledRectangleTool;

        public UILedEditorToolbar(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.LedEditorToolbar";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfRadioButton MoveTool
        {
            get
            {
                if (this.moveTool == null)
                {
                    this.moveTool = new WpfRadioButton(this);
                    this.moveTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_MoveTool";
                }

                return this.moveTool;
            }
        }

        public WpfRadioButton ZoomTool
        {
            get
            {
                if (this.zoomTool == null)
                {
                    this.zoomTool = new WpfRadioButton(this);
                    this.zoomTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_ZoomTool";
                }

                return this.zoomTool;
            }
        }

        public WpfRadioButton HandTool
        {
            get
            {
                if (this.handTool == null)
                {
                    this.handTool = new WpfRadioButton(this);
                    this.handTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_HandTool";
                }

                return this.handTool;
            }
        }

        public WpfRadioButton DynamicTextTool
        {
            get
            {
                if (this.dynamicTextTool == null)
                {
                    this.dynamicTextTool = new WpfRadioButton(this);
                    this.dynamicTextTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_DynamicTextTool";
                }

                return this.dynamicTextTool;
            }
        }

        public WpfRadioButton StaticTextTool
        {
            get
            {
                if (this.staticTextTool == null)
                {
                    this.staticTextTool = new WpfRadioButton(this);
                    this.staticTextTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_StaticTextTool";
                }

                return this.staticTextTool;
            }
        }

        public WpfRadioButton ImageTool
        {
            get
            {
                if (this.imageTool == null)
                {
                    this.imageTool = new WpfRadioButton(this);
                    this.imageTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_ImageTool";
                }

                return this.imageTool;
            }
        }

        public WpfRadioButton LedRectangleTool
        {
            get
            {
                if (this.ledRectangleTool == null)
                {
                    this.ledRectangleTool = new WpfRadioButton(this);
                    this.ledRectangleTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "LedEditorToolbar_LedRectangleTool";
                }

                return this.ledRectangleTool;
            }
        }
    }

    /* Filename: C:\Development\GorbaTfs\Gorba\Branches\Partners\Luminator\Center\Media\Source\WpfApplication.Tests\../../Source/Core/Views\Toolbars\TftEditorToolbar.xaml */
    public partial class UITftEditorToolbar : WpfCustom /* WPF Class = UserControl */
    {
        private WpfRadioButton moveTool;
        private WpfRadioButton zoomTool;
        private WpfRadioButton handTool;
        private WpfRadioButton dynamicTextTool;
        private WpfRadioButton staticTextTool;
        private WpfRadioButton imageTool;
        private WpfRadioButton videoTool;
        private WpfRadioButton imageList;
        private WpfRadioButton frameTool;
        private WpfRadioButton analogClock;

        public UITftEditorToolbar(UITestControl searchLimitContainer, string automationId = null)
            : base(searchLimitContainer)
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.TftEditorToolbar";
            if (!string.IsNullOrEmpty(automationId))
            {
                this.SearchProperties[WpfControl.PropertyNames.AutomationId] = automationId;
            }
        }

        public WpfRadioButton MoveTool
        {
            get
            {
                if (this.moveTool == null)
                {
                    this.moveTool = new WpfRadioButton(this);
                    this.moveTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_MoveTool";
                }

                return this.moveTool;
            }
        }

        public WpfRadioButton ZoomTool
        {
            get
            {
                if (this.zoomTool == null)
                {
                    this.zoomTool = new WpfRadioButton(this);
                    this.zoomTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_ZoomTool";
                }

                return this.zoomTool;
            }
        }

        public WpfRadioButton HandTool
        {
            get
            {
                if (this.handTool == null)
                {
                    this.handTool = new WpfRadioButton(this);
                    this.handTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_HandTool";
                }

                return this.handTool;
            }
        }

        public WpfRadioButton DynamicTextTool
        {
            get
            {
                if (this.dynamicTextTool == null)
                {
                    this.dynamicTextTool = new WpfRadioButton(this);
                    this.dynamicTextTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_DynamicTextTool";
                }

                return this.dynamicTextTool;
            }
        }

        public WpfRadioButton StaticTextTool
        {
            get
            {
                if (this.staticTextTool == null)
                {
                    this.staticTextTool = new WpfRadioButton(this);
                    this.staticTextTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_StaticTextTool";
                }

                return this.staticTextTool;
            }
        }

        public WpfRadioButton ImageTool
        {
            get
            {
                if (this.imageTool == null)
                {
                    this.imageTool = new WpfRadioButton(this);
                    this.imageTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_ImageTool";
                }

                return this.imageTool;
            }
        }

        public WpfRadioButton VideoTool
        {
            get
            {
                if (this.videoTool == null)
                {
                    this.videoTool = new WpfRadioButton(this);
                    this.videoTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_VideoTool";
                }

                return this.videoTool;
            }
        }

        public WpfRadioButton ImageList
        {
            get
            {
                if (this.imageList == null)
                {
                    this.imageList = new WpfRadioButton(this);
                    this.imageList.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_ImageList";
                }

                return this.imageList;
            }
        }

        public WpfRadioButton FrameTool
        {
            get
            {
                if (this.frameTool == null)
                {
                    this.frameTool = new WpfRadioButton(this);
                    this.frameTool.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_FrameTool";
                }

                return this.frameTool;
            }
        }

        public WpfRadioButton AnalogClock
        {
            get
            {
                if (this.analogClock == null)
                {
                    this.analogClock = new WpfRadioButton(this);
                    this.analogClock.SearchProperties[WpfControl.PropertyNames.AutomationId] = "EditorToolbar_AnalogClock";
                }

                return this.analogClock;
            }
        }
    }

}

