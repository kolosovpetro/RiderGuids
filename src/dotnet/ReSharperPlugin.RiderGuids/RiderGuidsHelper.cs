using System;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReSharperPlugin.RiderGuids;

public static class RiderGuidsHelper
{
    public static Action<ITextControl> AddGuidAfterToken(ICSharpContextActionDataProvider provider,
        string statementText)
    {
        var declaration = provider.GetSelectedElement<ICSharpStatement>();

        if (declaration == null)
            return null;

        using (WriteLockCookie.Create())
        {
            var elementFactory = CSharpElementFactory.GetInstance(declaration);
            var newToken = elementFactory.CreateStatement(statementText);

            var oldToken = provider.TokenAfterCaret.NotNull();
            newToken = ModificationUtil.AddChildAfter(oldToken, newToken);

            return x => x.Caret.MoveTo(newToken.GetDocumentEndOffset(), CaretVisualPlacement.DontScrollIfVisible);
        }
    }
}