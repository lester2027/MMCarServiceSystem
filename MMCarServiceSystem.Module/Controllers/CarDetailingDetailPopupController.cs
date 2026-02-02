using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using MMCarServiceSystem.Module.BusinessObjects;

namespace MMCarServiceSystem.Module.Controllers;

public class CarDetailingDetailPopupController : ViewController<ListView>
{
    public CarDetailingDetailPopupController()
    {
        TargetObjectType = typeof(CarDetailingDetail);
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
        {
            newController.NewObjectAction.Execute += NewObjectAction_Execute;
        }
    }

    protected override void OnDeactivated()
    {
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
        {
            newController.NewObjectAction.Execute -= NewObjectAction_Execute;
        }

        base.OnDeactivated();
    }

    private void NewObjectAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        e.ShowViewParameters.Context = TemplateContext.PopupWindow;
    }
}
