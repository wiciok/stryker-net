﻿using Stryker.Core.Exceptions;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Options
{
	public class SolutionPathOption : BaseStrykerOption<string>
	{

		public SolutionPathOption(string basePath, string solutionPath, IFileSystem fileSystem)
		{
			if (!string.IsNullOrWhiteSpace(basePath) && !string.IsNullOrWhiteSpace(solutionPath))
			{
				Value = FilePathUtils.NormalizePathSeparators(Path.Combine(basePath, solutionPath));
			}
            else
			{
				throw new StrykerInputException("SolutionPath needs two parameters: basePath, solutionPath");
			}

			if (fileSystem.File.Exists(Value))  //validate file existance and maintain moq
			{
				return;
			}
			else
			{
				throw new StrykerInputException("SolutionPath does not exist");
			}
		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string HelpText => @"Full path to your solution file. The solution file is needed to build the project and resolve dependencies for
    .net framework but can optionally be used for .net core. Path can be relative from test project or full path.";
		public override string DefaultValue => null;
	}
}