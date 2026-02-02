using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Editors;
using MMCarServiceSystem.Module.BusinessObjects;

namespace MMCarServiceSystem.Blazor.Server.Controllers
{
    public class PartPopupOnClickController
        : ObjectViewController<ListView, Part>
    {
        ListViewProcessCurrentObjectController processController;

        protected override void OnActivated()
        {
            base.OnActivated();

            processController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processController != null)
            {
                processController.CustomProcessSelectedItem += OpenPopup;
            }
        }

        private void OpenPopup(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;

            var os = Application.CreateObjectSpace(typeof(Appointment));
            var obj = os.GetObject(e.InnerArgs.CurrentObject);

            var detailView = Application.CreateDetailView(os, obj);
            detailView.ViewEditMode = ViewEditMode.Edit;

            var svp = new ShowViewParameters(detailView)
            {
                TargetWindow = TargetWindow.NewModalWindow
            };

            Application.ShowViewStrategy.ShowView(
                svp,
                new ShowViewSource(Frame, null)
            );
        }

        protected override void OnDeactivated()
        {
            if (processController != null)
            {
                processController.CustomProcessSelectedItem -= OpenPopup;
            }
            base.OnDeactivated();
        }
    }
}
