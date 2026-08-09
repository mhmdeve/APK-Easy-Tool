using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with the assembly.
[assembly: AssemblyTitle("APK Easy Tool")]
[assembly: AssemblyDescription("User friendly GUI APK Tool.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Evildog1")]
[assembly: AssemblyProduct("APK Easy Tool")]
[assembly: AssemblyCopyright("Copyright © 2022 Evildog1")]
[assembly: AssemblyTrademark("Evildog1")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("192b7a50-e0bd-48b0-9e15-aad634a5972a")]
[assembly: AssemblyVersion("1.6.1.0")]
[assembly: AssemblyFileVersion("1.6.1.0")]
[assembly: NeutralResourcesLanguage("en-US")]

namespace APKEasyTool
{
    /// <summary>
    /// Applies a modern visual system to the existing WinForms UI without
    /// changing the application's workflow or event handlers.
    /// </summary>
    internal static class ModernUi
    {
        private static readonly Color Background = Color.FromArgb(245, 247, 250);
        private static readonly Color Surface = Color.White;
        private static readonly Color Border = Color.FromArgb(224, 228, 235);
        private static readonly Color Text = Color.FromArgb(31, 41, 55);
        private static readonly Color Muted = Color.FromArgb(107, 114, 128);
        private static readonly Color Accent = Color.FromArgb(37, 99, 235);

        internal static void Apply(Form form)
        {
            if (form == null) return;

            form.SuspendLayout();
            form.BackColor = Background;
            form.ForeColor = Text;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.MinimumSize = new Size(980, 680);

            StyleTree(form);
            StyleTabs(form);
            StyleStatusBar(form);
            StyleLogo(form);

            form.ResumeLayout(true);
        }

        private static void StyleTree(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Button button = control as Button;
                TextBox textBox = control as TextBox;
                ComboBox comboBox = control as ComboBox;
                CheckBox checkBox = control as CheckBox;
                Label label = control as Label;
                RichTextBox richTextBox = control as RichTextBox;
                Panel panel = control as Panel;

                if (button != null)
                    StyleButton(button);
                else if (textBox != null)
                    StyleTextBox(textBox);
                else if (comboBox != null)
                    StyleComboBox(comboBox);
                else if (checkBox != null)
                    StyleCheckBox(checkBox);
                else if (label != null)
                    StyleLabel(label);
                else if (richTextBox != null)
                    StyleRichTextBox(richTextBox);
                else if (panel != null)
                {
                    panel.BackColor = Surface;
                    panel.Padding = new Padding(8);
                }

                if (control.HasChildren)
                    StyleTree(control);
            }
        }

        private static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.BackColor = Surface;
            button.ForeColor = Text;
            button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            button.Padding = new Padding(8, 2, 8, 2);

            button.MouseEnter += delegate
            {
                button.BackColor = Color.FromArgb(239, 246, 255);
                button.FlatAppearance.BorderColor = Accent;
            };

            button.MouseLeave += delegate
            {
                button.BackColor = Surface;
                button.FlatAppearance.BorderColor = Border;
            };
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Surface;
            textBox.ForeColor = Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Surface;
            comboBox.ForeColor = Text;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.ForeColor = Text;
            checkBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox.Cursor = Cursors.Hand;
        }

        private static void StyleLabel(Label label)
        {
            label.ForeColor = Text;
            if (label.Font.Size <= 9.5F)
                label.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static void StyleRichTextBox(RichTextBox box)
        {
            box.BackColor = Color.FromArgb(17, 24, 39);
            box.ForeColor = Color.FromArgb(229, 231, 235);
            box.BorderStyle = BorderStyle.None;
            box.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static void StyleTabs(Form form)
        {
            foreach (Control control in form.Controls)
            {
                TabControl tabs = control as TabControl;
                if (tabs == null) continue;

                tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabs.ItemSize = new Size(150, 36);
                tabs.SizeMode = TabSizeMode.Fixed;
                tabs.Padding = new Point(18, 5);
                tabs.DrawItem += DrawTab;

                foreach (TabPage page in tabs.TabPages)
                {
                    page.BackColor = Background;
                    page.ForeColor = Text;
                    page.Padding = new Padding(12);
                }
            }
        }

        private static void DrawTab(object sender, DrawItemEventArgs e)
        {
            TabControl tabs = (TabControl)sender;
            TabPage page = tabs.TabPages[e.Index];
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle bounds = e.Bounds;

            using (SolidBrush background = new SolidBrush(selected ? Surface : Background))
                e.Graphics.FillRectangle(background, bounds);

            if (selected)
            {
                using (SolidBrush accent = new SolidBrush(Accent))
                    e.Graphics.FillRectangle(accent, bounds.Left + 8, bounds.Bottom - 3, bounds.Width - 16, 3);
            }

            using (Font font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular, GraphicsUnit.Point))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    page.Text,
                    font,
                    bounds,
                    selected ? Accent : Muted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private static void StyleStatusBar(Form form)
        {
            foreach (Control control in form.Controls)
            {
                StatusStrip status = control as StatusStrip;
                if (status == null) continue;

                status.BackColor = Surface;
                status.ForeColor = Muted;
                status.SizingGrip = false;
            }
        }

        private static void StyleLogo(Form form)
        {
            foreach (Control control in form.Controls)
            {
                Panel panel = control as Panel;
                if (panel == null || panel.Name != "logoPanel") continue;

                panel.BackColor = Surface;
                panel.BorderStyle = BorderStyle.None;
            }
        }
    }
}
