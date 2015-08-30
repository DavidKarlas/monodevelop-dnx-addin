﻿//
// DependenciesNodeBuilderExtension.cs
//
// Author:
//       Matt Ward <ward.matt@gmail.com>
//
// Copyright (c) 2015 Matthew Ward
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.Dnx.NodeBuilders
{
	public class DependenciesNodeBuilderExtension : NodeBuilderExtension
	{
		DependenciesFolderNode currentFolderNode;

		public override bool CanBuildNode (Type dataType)
		{
			return typeof(DnxProject).IsAssignableFrom (dataType);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			Dispose ();

			var project = (DnxProject)dataObject;
			project.DependenciesChanged += ProjectDependenciesChanged;
			currentFolderNode = new DependenciesFolderNode (project);
			treeBuilder.AddChild (currentFolderNode);
		}

		void ProjectDependenciesChanged (object sender, EventArgs e)
		{
			ITreeBuilder builder = Context.GetTreeBuilder (currentFolderNode);
			if (builder != null)
				builder.UpdateChildren ();
		}

		public override void Dispose ()
		{
			if (currentFolderNode != null) {
				currentFolderNode.Project.DependenciesChanged -= ProjectDependenciesChanged;
			}
		}
	}
}
