namespace Sitecore.Support.Links
{
  using System.Collections.Generic;
  using Sitecore;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Links;

  /// <summary>
  /// Updates item version fields (<see cref="CustomField.UpdateLink"/>) that point at (linked to) given item, or any of its children. 
  /// <para>The <see cref="ItemLink"/> should be originated from a field, meaning <see cref="ItemLink.SourceFieldID"/> should be set.</para>
  /// <para>Updates links (<see cref="Sitecore.Links.LinkDatabase.UpdateReferences"/>) for all touched item versions.</para>
  /// </summary>
  public class LinkUpdaterJob
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkUpdaterJob" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public LinkUpdaterJob([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      this.ItemToUpdateLinksForSelfAndDescendants = item;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the item to update links for self and descendants.
    /// </summary>
    /// <value>
    /// The item to update links for self and descendants.
    /// </value>
    [NotNull]
    protected Item ItemToUpdateLinksForSelfAndDescendants { get; private set; }

    [NotNull]
    internal virtual LinkDatabase LinkDatabase
    {
      get
      {
        return Globals.LinkDatabase;
      }
    }


    #endregion

    #region Protected methods

    /// <summary>
    /// Updates the 
    /// </summary>
    [UsedImplicitly]
    protected virtual void Update()
    {
      var updatedItemVersions = new List<ItemUri>();
      this.UpdateLinksRecursively(this.ItemToUpdateLinksForSelfAndDescendants, updatedItemVersions);

      var linkDatabase = this.LinkDatabase;

      foreach (ItemUri uri in updatedItemVersions)
      {
        Item item = Database.GetItem(uri);

        if (item == null)
        {
          continue;
        }

        linkDatabase.UpdateReferences(item);
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Updates item version fields that contain links pointing at an <paramref name="item"/>, and keeps track of unique item versions processed.
    /// <para>Does same operation for all <paramref name="item"/> children - meaning will recursively call self for each item child.</para>
    /// </summary>
    /// <contract>
    ///   <requires name="item" condition="not null" />
    ///   <requires name="list" condition="not null" />
    /// </contract>
    private void UpdateLinksRecursively([NotNull]Item item, [NotNull] List<ItemUri> processedItems)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNull(processedItems, "processedItems");

      LinkDatabaseHelper.UpdateLink(item, processedItems);

      var children = item.Children;

      foreach (Item child in children)
      {
        this.UpdateLinksRecursively(child, processedItems);
      }
    }

    #endregion
  }
}
