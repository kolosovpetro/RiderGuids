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

namespace ReSharperPlugin.CodeInspections;

[ContextAction(
    Group = CSharpContextActions.GroupID,
    Name = "RiderGuids: Add Guid instance here",
    Description = "RiderGuids: Creates Guid instance in-place",
    Priority = -10)]
public class AddUpperCaseGuidStringContextAction : ContextActionBase
{
    private readonly ICSharpContextActionDataProvider _provider;

    public AddUpperCaseGuidStringContextAction(ICSharpContextActionDataProvider provider)
    {
        _provider = provider;
    }

    public override string Text => "RiderGuids: Add Guid string here (uppercase)";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        var p = _provider.GetSelectedElement<ICSharpStatement>();
        return p != null;
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var declaration = _provider.GetSelectedElement<ICSharpStatement>();

        if (declaration == null)
        {
            return null;
        }

        using (WriteLockCookie.Create())
        {
            var elementFactory = CSharpElementFactory.GetInstance(declaration);
            var newGuid = Guid.NewGuid().ToString().ToUpper();
            var statementText = $"var guidUpperString = \"{newGuid}\";";
            var newToken = elementFactory.CreateStatement(statementText);

            var oldToken = _provider.TokenAfterCaret.NotNull();
            newToken = ModificationUtil.AddChildAfter(oldToken, newToken);

            return x => x.Caret.MoveTo(newToken.GetDocumentEndOffset(), CaretVisualPlacement.DontScrollIfVisible);
        }
    }
}