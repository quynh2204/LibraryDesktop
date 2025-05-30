using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LibraryDesktop.Models;

namespace LibraryDesktop.View
{
    public partial class BookControl : UserControl
    {
        private LibraryDesktop.Models.Book? _book;
        public event EventHandler<LibraryDesktop.Models.Book>? BookClicked;

        // Parameterless constructor for designer support
        public BookControl()
        {
            InitializeComponent();
        }

        public BookControl(LibraryDesktop.Models.Book book)
        {
            _book = book;
            InitializeComponent();
            PopulateData();
        }

        public void SetBook(LibraryDesktop.Models.Book book)
        {
            _book = book;
            PopulateData();
        }

        private void PopulateData()
        {
            if (_book != null)
            {
                lblBookTitle.Text = _book.Title;
                lblAuthor.Text = $"{_book.Author}";
                lblChapterCount.Text = $"{_book.Chapters?.Count ?? 0} Chapters";
                
                // Show price panel if book has a price
                if (_book.Price > 0)
                {
                    panelPremium.Visible = true;
                    lblPrice.Text = $"{_book.Price} Coins";
                }
                else
                {
                    panelPremium.Visible = false;
                }                // Set book cover image if available
                if (!string.IsNullOrEmpty(_book.CoverImageUrl))
                {
                    try
                    {
                        string imagePath;
                        // Check if the URL already includes Assets folder
                        if (_book.CoverImageUrl.StartsWith("Assets/"))
                        {
                            imagePath = Path.Combine(Application.StartupPath, _book.CoverImageUrl);
                        }
                        else
                        {
                            imagePath = Path.Combine(Application.StartupPath, "Assets", _book.CoverImageUrl);
                        }
                        
                        if (File.Exists(imagePath))
                        {
                            picBookCover.Image = Image.FromFile(imagePath);
                        }
                    }
                    catch
                    {
                        // If image loading fails, keep default
                    }
                }
            }
        }

        private void BookControl_Click(object sender, EventArgs e)
        {
            if (_book != null)
            {
                BookClicked?.Invoke(this, _book);
            }
        }

        private void picBookCover_Click(object sender, EventArgs e)
        {
            BookControl_Click(sender, e);
        }        private void lblBookTitle_Click(object sender, EventArgs e)
        {
            BookControl_Click(sender, e);
        }
    }
}