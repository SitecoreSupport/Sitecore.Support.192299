namespace Sitecore.Support.Shell.Framework.Pipelines
{
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;
  using Sitecore.Web.UI.Sheer;

  public class RenameItem : Sitecore.Shell.Framework.Pipelines.RenameItem
  {
    public override void RepairLinks(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var itemToHaveLinksUpdated = GetItem(args);
      if (itemToHaveLinksUpdated == null)
      {
        base.RepairLinks(args);
        return;
      }

      JobOptions options = new JobOptions(
        jobName: "LinkUpdater",
        category: "LinkUpdater",
        siteName: Context.Site.Name,
        obj: new Support.Links.LinkUpdaterJob(itemToHaveLinksUpdated),
        methodName: "Update")
      {
        ContextUser = Context.User
      };

      JobManager.Start(options);
    }
  }
}