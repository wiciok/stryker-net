﻿using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify components.
    /// </summary>
    public interface IReadOnlyInputComponent
    {
        IEnumerable<IReadOnlyMutant> Mutants { get; }
        IParentComponent Parent { get; }
        string RelativePath { get; }
        string RelativePathToProjectFile { get; }
        string FullPath { get; }
        string Name { get; }

        IEnumerable<IReadOnlyMutant> ReadOnlyMutants { get; }
        IEnumerable<IReadOnlyMutant> TotalMutants { get; }
        IEnumerable<IReadOnlyMutant> DetectedMutants { get; }

        /// <summary>
        /// The display handlers are an exception to the readonly rule
        /// </summary>
        Display DisplayFile { get; set;  }
        Display DisplayFolder { get; set; }
        public void Display(int depth);

        double GetMutationScore();

        Health CheckHealth(Threshold threshold);
    }

    public delegate void Display(int depth, IReadOnlyInputComponent current);
}
