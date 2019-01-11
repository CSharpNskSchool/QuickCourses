using System;
using System.Windows.Forms;

namespace QuickCourses.CourseEditor
{
    using Api.Data.DataInterfaces;

    public partial class CourseEditorForm : Form
    {
        ICourseRepository courseRepository;
        IProgressRepository progressRepository;

        public CourseEditorForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            if (!LoginForm.TryAuth(
                this, 
                "mongodb://{0}:{1}@cluster0-shard-00-00-h88b4.mongodb.net:27017,cluster0-shard-00-01-h88b4.mongodb.net:27017,cluster0-shard-00-02-h88b4.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin",
                out courseRepository,
                out progressRepository))
            {
                Close();
            }

            base.OnShown(e);
        }
    }
}
