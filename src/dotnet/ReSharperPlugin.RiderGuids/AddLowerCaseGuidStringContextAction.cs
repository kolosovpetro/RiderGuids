using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperPlugin.RiderGuids;

namespace ReSharperPlugin.CodeInspections;

[ContextAction(
    Group = CSharpContextActions.GroupID,
    Name = "RiderGuids: Add Guid instance here",
    Description = "RiderGuids: Creates Guid instance in-place",
    Priority = -10)]
public class AddLowerCaseGuidStringContextAction : ContextActionBase
{
    private readonly ICSharpContextActionDataProvider _provider;

    public AddLowerCaseGuidStringContextAction(ICSharpContextActionDataProvider provider)
    {
        _provider = provider;
    }

    public override string Text => "RiderGuids: Add Guid string here (lowercase)";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        var p = _provider.GetSelectedElement<ICSharpStatement>();
        return p != null;
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var newGuid = Guid.NewGuid();
        var statementText = $"var guidLowerString = \"{newGuid}\";";

        var result = RiderGuidsHelper.AddGuidAfterToken(_provider, statementText);

        return result;
    }
}