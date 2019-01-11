using System.Windows.Forms;

namespace QuickCourses.CourseEditor
{
    using Api.Data.DataInterfaces;
    using Api.Data.Repositories;
    using Api.Data.Infrastructure;


    public partial class LoginForm : Form
    {
        public static bool TryAuth(
            IWin32Window window, 
            string connStringPattern,
            out ICourseRepository courseRepository,
            out IProgressRepository progressRepository)
        {
            var loginForm = new LoginForm(connStringPattern);
            var dialogResult = loginForm.ShowDialog();

            courseRepository = loginForm.courseRepository; ;
            progressRepository = loginForm.progressRepository;

            return dialogResult == DialogResult.OK;
        }

        private ICourseRepository courseRepository;
        private IProgressRepository progressRepository;
        private string connectionStringPattern;

        public LoginForm(string connectionStringPattern)
        {
            this.connectionStringPattern = connectionStringPattern;
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                var connectionString = string.Format(connectionStringPattern, textBox1.Text, textBox2.Text);
                courseRepository = new CourseRepository(
                    new Settings(
                        connectionString,
                        "QuickCourses",
                        "Course"
                        ));
                progressRepository = new ProgressRepository(
                    new Settings(
                        connectionString,
                        "QuickCourses",
                        "Progress"
                        ));
                DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show(this, "Data auth not valid.", "Can't connect");
            }
        }
    }
}
