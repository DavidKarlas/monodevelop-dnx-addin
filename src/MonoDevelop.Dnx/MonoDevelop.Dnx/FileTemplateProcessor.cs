﻿//
// FileTemplateProcessor.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc. (http://xamarin.com)
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
using System.IO;
using System.Linq;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Ide.Templates;
using MonoDevelop.Projects;

namespace MonoDevelop.Dnx
{
	public static class FileTemplateProcessor
	{
		static readonly FilePath templateSourceDirectory;

		static FileTemplateProcessor ()
		{
			templateSourceDirectory = GetTemplateSourceDirectory ();
		}

		public static void CreateFilesFromTemplate (Solution solution, DnxProject project, string projectTemplateName, params string[] files)
		{
			CreateFileFromTemplate (solution, "global.json");
			CreateFileFromTemplate (project, projectTemplateName, "project.json");

			foreach (string templateFileName in files) {
				CreateFileFromTemplate (project, projectTemplateName, templateFileName);
			}
		}

		public static void CreateFileFromTemplate (Project project, string projectTemplateName, string fileTemplateName)
		{
			if (!String.IsNullOrEmpty (projectTemplateName))
				fileTemplateName = projectTemplateName + "." + fileTemplateName;

			CreateFileFromTemplate (project, fileTemplateName);
		}

		public static void CreateFileFromTemplate (Project project, string fileTemplateName)
		{
			CreateFileFromTemplate (project, project.ParentSolution.RootFolder, fileTemplateName);
		}

		public static void CreateFileFromTemplate (Project project, SolutionItem policyItem, string fileTemplateName)
		{
			string templateFileName = templateSourceDirectory.Combine (fileTemplateName + ".xft.xml");
			using (Stream stream = File.OpenRead (templateFileName)) {
				var document = new XmlDocument ();
				document.Load (stream);

				foreach (XmlElement templateElement in document.DocumentElement["TemplateFiles"].ChildNodes.OfType<XmlElement> ()) {
					var template = FileDescriptionTemplate.CreateTemplate (templateElement, templateSourceDirectory);
					template.AddToProject (policyItem, project, "C#", project.BaseDirectory, null);
				}
			}
		}

		public static void CreateFileFromTemplate (Solution solution, string templateName)
		{
			var project = new GenericProject ();
			project.BaseDirectory = solution.BaseDirectory;
			CreateFileFromTemplate (project, solution.RootFolder, templateName);
		}

		static FilePath GetTemplateSourceDirectory ()
		{
			var assemblyPath = new FilePath (typeof(FileTemplateProcessor).Assembly.Location);
			return assemblyPath.ParentDirectory.Combine ("Templates", "Projects");
		}
	}
}
