using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        private static readonly Color AccentHover = Color.FromArgb(29, 78, 216);

        internal static Form Apply(Form form)
        {
            if (form == null) return null;

            form.SuspendLayout();
            form.BackColor = Background;
            form.ForeColor = Text;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MinimumSize = new Size(980, 680);

            StyleTree(form);
            StyleTabs(form);
            StyleStatusBar(form);
            StyleLogo(form);

            form.ResumeLayout(true);
            return form;
        }

        private static void StyleTree(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button button)
                {
                    StyleButton(button);
                }
                else if (control is TextBox textBox)
                {
                    StyleTextBox(textBox);
                }
                else if (control is ComboBox comboBox)
                {
                    StyleComboBox(comboBox);
                }
                else if (control is CheckBox checkBox)
                {
                    StyleCheckBox(checkBox);
                }
                else if (control is Label label)
                {
                    StyleLabel(label);
                }
                else if (control is RichTextBox richTextBox)
                {
                    StyleRichTextBox(richTextBox);
                }
                else if (control is Panel panel)
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
            button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            button.Padding = new Padding(8, 2, 8, 2);

            button.MouseEnter += delegate { button.BackColor = Color.FromArgb(239, 246, 255); button.FlatAppearance.BorderColor = Accent; };
            button.MouseLeave += delegate { button.BackColor = Surface; button.FlatAppearance.BorderColor = Border; };
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Surface;
            textBox.ForeColor = Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 9F);
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Surface;
            comboBox.ForeColor = Text;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 9F);
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.ForeColor = Text;
            checkBox.Font = new Font("Segoe UI", 9F);
            checkBox.Cursor = Cursors.Hand;
        }

        private static void StyleLabel(Label label)
        {
            label.ForeColor = Text;
            if (label.Font.Size <= 9.5F)
                label.Font = new Font("Segoe UI", 9F);
        }

        private static void StyleRichTextBox(RichTextBox box)
        {
            box.BackColor = Color.FromArgb(17, 24, 39);
            box.ForeColor = Color.FromArgb(229, 231, 235);
            box.BorderStyle = BorderStyle.None;
            box.Font = new Font("Cascadia Mono", 9F);
        }

        private static void StyleTabs(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (!(control is TabControl tabs)) continue;

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
            var tabs = (TabControl)sender;
            var page = tabs.TabPages[e.Index];
            var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var bounds = e.Bounds;

            using (var background = new SolidBrush(selected ? Surface : Background))
                e.Graphics.FillRectangle(background, bounds);

            if (selected)
            {
                using (var accent = new SolidBrush(Accent))
                    e.Graphics.FillRectangle(accent, bounds.Left + 8, bounds.Bottom - 3, bounds.Width - 16, 3);
            }

            TextRenderer.DrawText(
                e.Graphics,
                page.Text,
                new Font("Segoe UI Semibold", 9F),
                bounds,
                selected ? Accent : Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static void StyleStatusBar(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (!(control is StatusStrip status)) continue;
                status.BackColor = Surface;
                status.ForeColor = Muted;
                status.SizingGrip = false;
            }
        }

        private static void StyleLogo(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (!(control is Panel panel)) continue;
                if (panel.Name != "logoPanel") continue;

                panel.BackColor = Surface;
                panel.BorderStyle = BorderStyle.None;
            }
        }
    }
}
