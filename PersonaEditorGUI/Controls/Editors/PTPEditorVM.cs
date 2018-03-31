﻿using PersonaEditorLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonaEditorLib.FileStructure.Text;
using System.IO;
using System.Windows.Data;
using System.Globalization;
using PersonaEditorLib.Extension;
using System.Windows.Media.Imaging;
using PersonaEditorGUI.Classes.Media.Visual;
using System.Windows;
using System.Windows.Media;
using PersonaEditorLib.Interfaces;

namespace PersonaEditorGUI.Controls.Editors
{
    class PTPNameEditVM : BindingObject
    {
        PTPName name;
        private string _OldName = "";
        private string OldEncoding;

        public int Index => name.Index;
        public string OldName => _OldName;
        public string NewName
        {
            get { return name.NewName; }
            set
            {
                if (name.NewName != value)
                {
                    name.NewName = value;
                    Notify("NewName");
                }
            }
        }

        public void UpdateOldEncoding(string OldEncoding)
        {
            this.OldEncoding = OldEncoding;
            _OldName = name.OldName.GetTextBaseList().GetString(Static.EncodingManager.GetPersonaEncoding(OldEncoding));
            Notify("OldName");
        }

        public PTPNameEditVM(PTPName name, string OldEncoding)
        {
            this.name = name;
            this.OldEncoding = OldEncoding;
            _OldName = name.OldName.GetTextBaseList().GetString(Static.EncodingManager.GetPersonaEncoding(OldEncoding));
        }
    }

    class PTPMsgStrEditVM : BindingObject
    {
        TextVisual OldText;
        TextVisual NewText;

        BackgroundImage BackgroundImg;
        EventWrapper BackgroundEW;

        private string OldEncoding;
        private string NewEncoding;

        MSG.MSGstr str;
        EventWrapper strEW;

        public string Prefix
        {
            get { return MSGListToSystem(str.Prefix); }
        }
        public string Postfix
        {
            get { return MSGListToSystem(str.Postfix); }
        }
        public string OldString
        {
            get { return str.OldString.GetString(Static.EncodingManager.GetPersonaEncoding(OldEncoding), true); }
        }
        public string NewString
        {
            get { return str.NewString; }
            set { str.NewString = value; }
        }

        public BitmapSource BackgroundImage => BackgroundImg.Image;
        public Rect BackgroundRect => BackgroundImg.Rect;

        private ImageSource oldTextSource = null;
        private Rect oldTextRect;
        public ImageSource OldTextSource
        {
            get { return oldTextSource; }
            set
            {
                if (oldTextSource != value)
                {
                    oldTextSource = value;
                    Notify("OldTextSource");
                }
            }
        }
        public Rect OldTextRect
        {
            get { return oldTextRect; }
            set
            {
                if (oldTextRect != value)
                {
                    oldTextRect = value;
                    Notify("OldTextRect");
                }
            }
        }

        private ImageSource oldNameSource = null;
        private Rect oldNameRect;
        public ImageSource OldNameSource
        {
            get { return oldNameSource; }
            set
            {
                if (oldNameSource != value)
                {
                    oldNameSource = value;
                    Notify("OldNameSource");
                }
            }
        }
        public Rect OldNameRect
        {
            get { return oldNameRect; }
            set
            {
                if (oldNameRect != value)
                {
                    oldNameRect = value;
                    Notify("OldNameRect");
                }
            }
        }

        private ImageSource newTextSource = null;
        private Rect newTextRect;
        public ImageSource NewTextSource
        {
            get { return newTextSource; }
            set
            {
                if (newTextSource != value)
                {
                    newTextSource = value;
                    Notify("NewTextSource");
                }
            }
        }
        public Rect NewTextRect
        {
            get { return newTextRect; }
            set
            {
                if (newTextRect != value)
                {
                    newTextRect = value;
                    Notify("NewTextRect");
                }
            }
        }

        private ImageSource newNameSource = null;
        private Rect newNameRect;
        public ImageSource NewNameSource
        {
            get { return newNameSource; }
            set
            {
                if (newNameSource != value)
                {
                    newNameSource = value;
                    Notify("NewNameSource");
                }
            }
        }
        public Rect NewNameRect
        {
            get { return newNameRect; }
            set
            {
                if (newNameRect != value)
                {
                    newNameRect = value;
                    Notify("NewNameRect");
                }
            }
        }

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MSG.MSGstr Msg)
            {
                if (e.PropertyName == "NewString")
                    NewText.UpdateText(str.NewString.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)));
            }
            else if (sender is BackgroundImage image)
            {
                if (e.PropertyName == "TextStart")
                {
                    OldText.Start = image.TextStart;
                    NewText.Start = image.TextStart;
                }
                else if (e.PropertyName == "ColorText")
                {
                    OldText.Color = image.ColorText;
                    NewText.Color = image.ColorText;
                }
                else if (e.PropertyName == "LineSpacing")
                {
                    OldText.LineSpacing = image.LineSpacing;
                    NewText.LineSpacing = image.LineSpacing;
                }
                else if (e.PropertyName == "GlyphScale")
                {
                    OldText.GlyphScale = image.GlyphScale;
                    NewText.GlyphScale = image.GlyphScale;
                }
            }
        }

        public void UpdateOldEncoding(string OldEncoding)
        {
            this.OldEncoding = OldEncoding;
            OldText.UpdateFont(Static.FontManager.GetPersonaFont(OldEncoding));
            Notify("OldString");
        }

        public void UpdateNewEncoding(string NewEncoding)
        {
            this.NewEncoding = NewEncoding;
            NewText.UpdateText(str.NewString.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)), Static.FontManager.GetPersonaFont(NewEncoding));
        }

        public void UpdateBackground()
        {
            Notify("BackgroundImage");
            Notify("BackgroundRect");
        }

        public PTPMsgStrEditVM(MSG.MSGstr str, string OldEncoding, string NewEncoding, BackgroundImage backgroundImage)
        {
            this.str = str;
            this.OldEncoding = OldEncoding;
            this.NewEncoding = NewEncoding;

            strEW = new EventWrapper(str, this);

            BackgroundImg = backgroundImage;
            BackgroundEW = new EventWrapper(backgroundImage, this);

            OldText = new TextVisual(Static.FontManager.GetPersonaFont(OldEncoding)) { Tag = "Old" };
            NewText = new TextVisual(Static.FontManager.GetPersonaFont(NewEncoding)) { Tag = "New" };
            OldText.VisualChanged += OldText_VisualChanged;
            NewText.VisualChanged += NewText_VisualChanged;

            SetBack(backgroundImage);

            OldText.UpdateText(str.OldString);
            NewText.UpdateText(str.NewString.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)));
        }

        public void OldName_VisualChanged(ImageSource imageSource, Rect rect)
        {
            OldNameSource = imageSource;
            OldNameRect = rect;
        }

        public void NewName_VisualChanged(ImageSource imageSource, Rect rect)
        {
            NewNameSource = imageSource;
            NewNameRect = rect;
        }

        private void OldText_VisualChanged(ImageSource imageSource, Rect rect)
        {
            OldTextSource = imageSource;
            OldTextRect = rect;
        }

        private void NewText_VisualChanged(ImageSource imageSource, Rect rect)
        {
            NewTextSource = imageSource;
            NewTextRect = rect;
        }

        string MSGListToSystem(IList<TextBaseElement> list)
        {
            string returned = "";
            foreach (var Bytes in list)
            {
                byte[] temp = Bytes.Array.ToArray();
                if (temp.Length > 0)
                {
                    returned += "{" + System.Convert.ToString(temp[0], 16).PadLeft(2, '0').ToUpper();
                    for (int i = 1; i < temp.Length; i++)
                    {
                        returned += "\u00A0" + System.Convert.ToString(temp[i], 16).PadLeft(2, '0').ToUpper();
                    }
                    returned += "} ";
                }
            }
            return returned;
        }

        private void SetBack(BackgroundImage image)
        {
            OldText.Start = image.TextStart;
            NewText.Start = image.TextStart;

            OldText.Color = image.ColorText;
            NewText.Color = image.ColorText;

            OldText.LineSpacing = image.LineSpacing;
            NewText.LineSpacing = image.LineSpacing;

            OldText.GlyphScale = image.GlyphScale;
            NewText.GlyphScale = image.GlyphScale;
        }
    }

    class PTPMsgVM : BindingObject
    {
        MSG msg;
        PTPName name;
        private string NewEncoding;

        EventWrapper BackgroundEW;
        EventWrapper PTPNameEW;

        TextVisual OldName;
        TextVisual NewName;

        public string Name => msg.Name;

        public ObservableCollection<PTPMsgStrEditVM> Strings { get; } = new ObservableCollection<PTPMsgStrEditVM>();

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is PTPName name)
            {
                if (e.PropertyName == "NewName")
                    NewName.UpdateText(name.NewName.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)));
                else if (e.PropertyName == "OldName")
                    OldName.UpdateText(name.OldName);
            }
            else if (sender is BackgroundImage image && OldName != null && NewName != null)
            {
                if (e.PropertyName == "NameStart")
                {
                    OldName.Start = image.NameStart;
                    NewName.Start = image.NameStart;
                }
                else if (e.PropertyName == "ColorName")
                {
                    OldName.Color = image.ColorName;
                    NewName.Color = image.ColorName;
                }
                else if (e.PropertyName == "LineSpacing")
                {
                    OldName.LineSpacing = image.LineSpacing;
                    NewName.LineSpacing = image.LineSpacing;
                }
                else if (e.PropertyName == "GlyphScale")
                {
                    OldName.GlyphScale = image.GlyphScale;
                    NewName.GlyphScale = image.GlyphScale;
                }
            }
        }

        public void UpdateOldEncoding(string OldEncoding)
        {
            OldName?.UpdateFont(Static.FontManager.GetPersonaFont(OldEncoding));
            foreach (var a in Strings)
                a.UpdateOldEncoding(OldEncoding);
        }

        public void UpdateNewEncoding(string NewEncoding)
        {
            this.NewEncoding = NewEncoding;
            if (name != null)
                NewName?.UpdateText(name.NewName.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)));
            NewName?.UpdateFont(Static.FontManager.GetPersonaFont(NewEncoding));
            foreach (var a in Strings)
                a.UpdateNewEncoding(NewEncoding);
        }

        public void UpdateBackground()
        {
            foreach (var a in Strings)
                a.UpdateBackground();
        }

        public PTPMsgVM(MSG msg, PTPName name, string OldEncoding, string NewEncoding, BackgroundImage backgroundImage)
        {
            this.msg = msg;
            this.name = name;
            this.NewEncoding = NewEncoding;

            BackgroundEW = new EventWrapper(backgroundImage, this);

            foreach (var a in msg.Strings)
                Strings.Add(new PTPMsgStrEditVM(a, OldEncoding, NewEncoding, backgroundImage));

            if (name != null)
            {
                OldName = new TextVisual(Static.FontManager.GetPersonaFont(OldEncoding)) { Tag = "Old" };
                NewName = new TextVisual(Static.FontManager.GetPersonaFont(NewEncoding)) { Tag = "New" };
                SetBack(backgroundImage);
                OldName.VisualChanged += OldName_VisualChanged;
                NewName.VisualChanged += NewName_VisualChanged;

                OldName.UpdateText(name.OldName);
                NewName.UpdateText(name.NewName.GetTextBaseList(Static.EncodingManager.GetPersonaEncoding(NewEncoding)));
                PTPNameEW = new EventWrapper(name, this);
            }
        }

        private void OldName_VisualChanged(ImageSource imageSource, Rect rect)
        {
            foreach (var a in Strings)
                a.OldName_VisualChanged(imageSource, rect);
        }

        private void NewName_VisualChanged(ImageSource imageSource, Rect rect)
        {
            foreach (var a in Strings)
                a.NewName_VisualChanged(imageSource, rect);
        }

        private void SetBack(BackgroundImage image)
        {
            OldName.Start = image.NameStart;
            NewName.Start = image.NameStart;

            OldName.Color = image.ColorName;
            NewName.Color = image.ColorName;

            OldName.LineSpacing = image.LineSpacing;
            NewName.LineSpacing = image.LineSpacing;

            OldName.GlyphScale = image.GlyphScale;
            NewName.GlyphScale = image.GlyphScale;
        }
    }

    class PTPEditorVM : BindingObject, IViewModel
    {
        #region Private

        EventWrapper BackgroundEW;
        EventWrapper EncodingManagerEW;
        Backgrounds BackImage { get; } = new Backgrounds(Static.Paths.DirBackgrounds);

        #endregion Private

        public ReadOnlyObservableCollection<string> BackgroundList => BackImage.BackgroundList;
        public int SelectedIndex
        {
            get { return BackImage.SelectedIndex; }
            set
            {
                BackImage.SelectedIndex = value;
                Settings.AppSetting.Default.PTPBackgroundDefault = BackImage.SelectedItem;
            }
        }

        private int OldEncoding;
        private int NewEncoding;

        public ReadOnlyObservableCollection<string> FontList => Static.EncodingManager.EncodingList;

        public int SelectedOldFont
        {
            get { return OldEncoding; }
            set
            {
                OldEncoding = value;
                Settings.AppSetting.Default.PTPOldDefault = Static.EncodingManager.GetPersonaEncodingName(value);
                UpdateOldEncoding(Settings.AppSetting.Default.PTPOldDefault);
                Notify("SelectedOldFont");
            }
        }

        public int SelectedNewFont
        {
            get { return NewEncoding; }
            set
            {
                NewEncoding = value;
                Settings.AppSetting.Default.PTPNewDefault = Static.EncodingManager.GetPersonaEncodingName(value);
                UpdateNewEncoding(Settings.AppSetting.Default.PTPNewDefault);
                Notify("SelectedNewFont");
            }
        }

        public ObservableCollection<PTPNameEditVM> Names { get; } = new ObservableCollection<PTPNameEditVM>();
        public ObservableCollection<PTPMsgVM> MSG { get; } = new ObservableCollection<PTPMsgVM>();

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is BackgroundImage image)
            {
                if (e.PropertyName == "Image" | e.PropertyName == "Rect")
                    UpdateBackground();
            }
            else if (sender is PersonaEditorLib.PersonaEncoding.PersonaEncodingManager man)
            {
                if (e.PropertyName == man.GetPersonaEncodingName(OldEncoding))
                    UpdateOldEncoding(Settings.AppSetting.Default.PTPOldDefault);
                if (e.PropertyName == man.GetPersonaEncodingName(NewEncoding))
                    UpdateNewEncoding(Settings.AppSetting.Default.PTPNewDefault);
            }
        }

        public void UpdateOldEncoding(string OldEncoding)
        {
            foreach (var a in Names)
                a.UpdateOldEncoding(OldEncoding);
            foreach (var a in MSG)
                a.UpdateOldEncoding(OldEncoding);
        }

        public void UpdateNewEncoding(string NewEncoding)
        {
            foreach (var a in MSG)
                a.UpdateNewEncoding(NewEncoding);
        }

        public void UpdateBackground()
        {
            foreach (var a in MSG)
                a.UpdateBackground();
        }

        public PTPEditorVM(PTP ptp)
        {
            BackImage.SelectedItem = Settings.AppSetting.Default.PTPBackgroundDefault;

            int sourceInd = Static.EncodingManager.GetPersonaEncodingIndex(Settings.AppSetting.Default.PTPOldDefault);
            if (sourceInd >= 0)
                OldEncoding = sourceInd;
            else
                OldEncoding = 0;

            sourceInd = Static.EncodingManager.GetPersonaEncodingIndex(Settings.AppSetting.Default.PTPNewDefault);
            if (sourceInd >= 0)
                NewEncoding = sourceInd;
            else
                NewEncoding = 0;

            BackgroundEW = new EventWrapper(BackImage.CurrentBackground, this);
            EncodingManagerEW = new EventWrapper(Static.EncodingManager, this);

            foreach (var a in ptp.names)
                Names.Add(new PTPNameEditVM(a, Settings.AppSetting.Default.PTPOldDefault));

            foreach (var a in ptp.msg)
                MSG.Add(new PTPMsgVM(a, ptp.names.FirstOrDefault(x => x.Index == a.CharacterIndex), Settings.AppSetting.Default.PTPOldDefault, Settings.AppSetting.Default.PTPNewDefault, BackImage.CurrentBackground));
        }

        public bool Close()
        {
            return true;
        }
    }
}