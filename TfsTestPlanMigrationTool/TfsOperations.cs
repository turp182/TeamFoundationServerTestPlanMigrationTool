//The MIT License (MIT)

//Copyright (c) 20015 Jason Wilbur Turpin

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the Software), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.Framework.Client;
using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Diagnostics;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Framework.Common;

namespace TFSTestMigrationTool
{
    /// <summary>
    /// TfsOperations contains various TFS methods for reading or creating TFS Test related artifacts.
    /// </summary>
    public static class TfsOperations
    {
        private static ICommonStructureService _destinationStructureService = null;

        private static List<Node> _destinationAreaNodes = new List<Node>();
        private static List<Node> _destinationIterationNodes = new List<Node>();

        /// <summary>
        /// Opens the Common TFS Project Selection Form.
        /// </summary>
        /// <returns></returns>
        public static ITestManagementTeamProject SelectTfsProject(bool isDestinationProject)
        {
            TeamProjectPicker teamProjectPicker = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);
            DialogResult result = teamProjectPicker.ShowDialog();

            if (result == DialogResult.OK && teamProjectPicker.SelectedTeamProjectCollection != null)
            {
                if (isDestinationProject && teamProjectPicker.SelectedTeamProjectCollection != null)
                {
                    TfsTeamProjectCollection destinationTeamProjectCollection = teamProjectPicker.SelectedTeamProjectCollection;
                    destinationTeamProjectCollection.Connect(ConnectOptions.IncludeServices);
                    _destinationStructureService = destinationTeamProjectCollection.GetService(typeof(ICommonStructureService)) as ICommonStructureService;
                }

                TfsTeamProjectCollection teamProjectCollection = teamProjectPicker.SelectedTeamProjectCollection;
                ITestManagementService testManagementService = (ITestManagementService)teamProjectCollection.GetService(typeof(ITestManagementService));

                ITestManagementTeamProject project = testManagementService.GetTeamProject(teamProjectPicker.SelectedProjects[0].Name);

                return project;
            }
            return null;
        }

        /// <summary>
        /// Gets the test plans for TFS project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static ITestPlanCollection GetTestPlansForTfsProject(ITestManagementTeamProject project)
        {
            ITestPlanCollection retVal = project.TestPlans.Query("Select * From TestPlan");

            return retVal;
        }

        /// <summary>
        /// Copies a test plan.
        /// </summary>
        /// <param name="destinationProject">The destination project.</param>
        /// <param name="sourceTestPlan">The source test plan.</param>
        public static string CopyTestPlan(ITestManagementTeamProject destinationProject, ITestPlan sourceTestPlan)
        {
            string results = "Copying Test Plan " + sourceTestPlan.Name + Environment.NewLine;

            CreateAndCollectInfoForDestinationAreaAndIterations(destinationProject, sourceTestPlan.Project.WitProject);

            ITestPlan destinationTestPlan = destinationProject.TestPlans.Create();
            destinationTestPlan.Name = sourceTestPlan.Name;
            destinationTestPlan.StartDate = sourceTestPlan.StartDate;
            destinationTestPlan.EndDate = sourceTestPlan.EndDate;
            destinationTestPlan.Save();

            results += CopyTestCases(sourceTestPlan.RootSuite, destinationTestPlan.RootSuite);

            results += CopyTestSuites(sourceTestPlan, destinationTestPlan);

            return results;
        }
        private static string CreateAndCollectInfoForDestinationAreaAndIterations(ITestManagementTeamProject destinationTestProject, Project sourceWitProject)
        {
            if (_destinationStructureService == null)
            {
                return "******** Couldn't connect to the Destination Structure Service, cannot create Areas or Iterations" + Environment.NewLine;
            }

            string rootAreaNodePath = string.Format("\\{0}\\Area", destinationTestProject.TeamProjectName);
            NodeInfo areaPathRootInfo = _destinationStructureService.GetNodeFromPath(rootAreaNodePath);
            _destinationAreaNodes.Clear();

            RecurseAreas(sourceWitProject.AreaRootNodes, areaPathRootInfo, destinationTestProject.WitProject.AreaRootNodes, destinationTestProject.WitProject);

            string rootIterationNodePath = string.Format("\\{0}\\Iteration", destinationTestProject.TeamProjectName);
            NodeInfo iterationPathRootInfo = _destinationStructureService.GetNodeFromPath(rootIterationNodePath);
            _destinationIterationNodes.Clear();

            RecurseIterations(sourceWitProject.IterationRootNodes, iterationPathRootInfo, destinationTestProject.WitProject.IterationRootNodes, destinationTestProject.WitProject);

            return string.Empty;
        }

        #region Area Node Methods
        private static void RecurseAreas(NodeCollection sourceNodes, NodeInfo destinationRootNodeInfo, NodeCollection destinationRootNodes, Project destinationWitProject)
        {
            foreach (Node sourceArea in sourceNodes)
            {
                NodeInfo destAreaNodeInfo = null;
                Node destAreaNode = null;

                if (destinationRootNodes.Cast<Node>().FirstOrDefault(n => n.Name == sourceArea.Name) != null)
                {
                    destAreaNode = destinationRootNodes.Cast<Node>().FirstOrDefault(n => n.Name == sourceArea.Name);
                    destAreaNodeInfo = _destinationStructureService.GetNode(destAreaNode.Uri.ToString());

                    _destinationAreaNodes.Add(destAreaNode);
                }

                if (destAreaNodeInfo == null) // node doesn't exist
                {
                    string newAreaNodeUri = _destinationStructureService.CreateNode(sourceArea.Name, destinationRootNodeInfo.Uri);
                    destAreaNodeInfo = _destinationStructureService.GetNode(newAreaNodeUri);
                    destAreaNode = FindAreaNode(destinationWitProject, destAreaNodeInfo.Path);

                    _destinationAreaNodes.Add(destAreaNode);
                }

                if (sourceArea.ChildNodes.Count > 0)
                {
                    RecurseAreas(sourceArea.ChildNodes, destAreaNodeInfo, destAreaNode.ChildNodes, destinationWitProject);
                }
            }
        }

        private static Node GetRootAreaNodeByName(Project project, string name)
        {
            Node rootNode = null;

            bool areaFound = false;
            while (!areaFound)
            {
                try
                {
                    project.Store.RefreshCache(true);
                    project.Store.SyncToCache();

                    Node tempNode = project.Store.Projects[project.Name].AreaRootNodes[name];
                    rootNode = tempNode;  // if it isn't found an exception is thrown never getting to this.                            
                    areaFound = true;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(1000);
                    areaFound = false;
                }
            }
            return rootNode;
        }
        private static Node FindAreaNode(Project project, string path)
        {
            path = path.Substring(1); // remove leading //

            List<string> pathParts = path.Split('\\').ToList();
            string projectName = pathParts[0];
            pathParts.RemoveAt(0); // remove the project name from the path parts
            pathParts.RemoveAt(0); // remove the word Area
            string rootAreaNodeName = pathParts[0];
            pathParts.RemoveAt(0); // remove the root node name

            Node currentNode = GetRootAreaNodeByName(project, rootAreaNodeName);

            for (int index = 0; index < pathParts.Count; index++)
            {
                bool areaFound = false;
                while (!areaFound)
                {
                    try
                    {
                        project.Store.RefreshCache(true);
                        project.Store.SyncToCache();

                        string nodeName = pathParts[index];

                        Node tempNode = currentNode.ChildNodes[nodeName];
                        currentNode = tempNode;  // if it isn't found an exception is thrown never getting to this.                            
                        areaFound = true;
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Thread.Sleep(1000);
                        currentNode = GetRootAreaNodeByName(project, rootAreaNodeName);
                        areaFound = false;
                        break;
                    }
                }
            }

            return currentNode;
        }

        #endregion

        #region Iteration Node Methods

        private static void RecurseIterations(NodeCollection sourceNodes, NodeInfo destinationRootNodeInfo, NodeCollection destinationRootNodes, Project destinationWitProject)
        {
            foreach (Node sourceIteration in sourceNodes)
            {
                NodeInfo destIterationNodeInfo = null;
                Node destIterationNode = null;

                if (destinationRootNodes.Cast<Node>().FirstOrDefault(n => n.Name == sourceIteration.Name) != null)
                {
                    destIterationNode = destinationRootNodes.Cast<Node>().FirstOrDefault(n => n.Name == sourceIteration.Name);
                    destIterationNodeInfo = _destinationStructureService.GetNode(destIterationNode.Uri.ToString());

                    _destinationIterationNodes.Add(destIterationNode);
                }

                if (destIterationNodeInfo == null) // node doesn't exist
                {
                    string newAreaNodeUri = _destinationStructureService.CreateNode(sourceIteration.Name, destinationRootNodeInfo.Uri);
                    destIterationNodeInfo = _destinationStructureService.GetNode(newAreaNodeUri);
                    destIterationNode = FindIterationNode(destinationWitProject, destIterationNodeInfo.Path);

                    _destinationIterationNodes.Add(destIterationNode);
                }

                if (sourceIteration.ChildNodes.Count > 0)
                {
                    RecurseIterations(sourceIteration.ChildNodes, destIterationNodeInfo, destIterationNode.ChildNodes, destinationWitProject);
                }
            }
        }

        private static Node FindIterationNode(Project project, string path)
        {
            path = path.Substring(1); // remove leading //

            List<string> pathParts = path.Split('\\').ToList();
            string projectName = pathParts[0];
            pathParts.RemoveAt(0); // remove the project name from the path parts
            pathParts.RemoveAt(0); // remove the word Iteration
            string rootIterationNodeName = pathParts[0];
            pathParts.RemoveAt(0); // remove the root node name

            Node currentNode = GetRootIterationNodeByName(project, rootIterationNodeName);

            for (int index = 0; index < pathParts.Count; index++)
            {
                bool iterationFound = false;
                while (!iterationFound)
                {
                    try
                    {
                        project.Store.RefreshCache(true);
                        project.Store.SyncToCache();

                        string nodeName = pathParts[index];

                        Node tempNode = currentNode.ChildNodes[nodeName];
                        currentNode = tempNode;  // if it isn't found an exception is thrown never getting to this.                            
                        iterationFound = true;
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Thread.Sleep(1000);
                        currentNode = GetRootIterationNodeByName(project, rootIterationNodeName);
                        iterationFound = false;
                        break;
                    }
                }
            }

            return currentNode;
        }

        private static Node GetRootIterationNodeByName(Project project, string name)
        {
            Node rootNode = null;

            bool areaFound = false;
            while (!areaFound)
            {
                try
                {
                    project.Store.RefreshCache(true);
                    project.Store.SyncToCache();

                    Node tempNode = project.Store.Projects[project.Name].IterationRootNodes[name];
                    rootNode = tempNode;  // if it isn't found an exception is thrown never getting to this.                            
                    areaFound = true;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(1000);
                    areaFound = false;
                }
            }
            return rootNode;
        }

        #endregion

        /// <summary>
        /// Copies test suites located at the root of a test plan.
        /// </summary>
        /// <param name="sourceTestPlan">The source test plan.</param>
        /// <param name="destinationTestPlan">The destination test plan.</param>
        private static string CopyTestSuites(ITestPlan sourceTestPlan, ITestPlan destinationTestPlan)
        {
            string results = string.Empty;

            foreach (IStaticTestSuite sourceTestSuite in sourceTestPlan.RootSuite.SubSuites)
            {
                IStaticTestSuite destinationTestSuite = destinationTestPlan.Project.TestSuites.CreateStatic();

                destinationTestSuite.Title = sourceTestSuite.Title;
                destinationTestSuite.SetDefaultConfigurations(GetDefaultConfigurationCollection(destinationTestPlan.Project));
                destinationTestPlan.RootSuite.Entries.Add(destinationTestSuite);
                destinationTestPlan.Save();

                results += CopyTestCases(sourceTestSuite, destinationTestSuite);

                if (sourceTestSuite.Entries.Count > 0)
                {
                    results += CopyTestSuites(sourceTestSuite, destinationTestSuite);
                }
            }
            return results;
        }

        /// <summary>
        /// Copies test suites that are children of another test suite.
        /// </summary>
        /// <param name="parentSourceTestSuite">The parent source test suite.</param>
        /// <param name="parentDestinationTestSuite">The parent destination test suite.</param>
        private static string CopyTestSuites(IStaticTestSuite parentSourceTestSuite, IStaticTestSuite parentDestinationTestSuite)
        {
            string results = string.Empty;

            foreach (IStaticTestSuite sourceTestSuite in parentSourceTestSuite.SubSuites)
            {
                IStaticTestSuite destinationTestSuite = parentDestinationTestSuite.Project.TestSuites.CreateStatic();

                destinationTestSuite.Title = sourceTestSuite.Title;
                destinationTestSuite.SetDefaultConfigurations(GetDefaultConfigurationCollection(parentDestinationTestSuite.Project));
                parentDestinationTestSuite.Entries.Add(destinationTestSuite);

                parentDestinationTestSuite.Plan.Save();

                results = "Copying Test Suite: " + sourceTestSuite.Title + Environment.NewLine;

                results += CopyTestCases(sourceTestSuite, destinationTestSuite);

                if (sourceTestSuite.Entries.Count > 0)
                {
                    results += CopyTestSuites(sourceTestSuite, destinationTestSuite);
                }
            }
            return results;
        }

        /// <summary>
        /// Copies test cases associated with a test suite (also work for test plan rooted test cases using Plan.RootSuite).
        /// </summary>
        /// <param name="sourceTestSuite">The source test suite.</param>
        /// <param name="destinationTestSuite">The destination test suite.</param>
        private static string CopyTestCases(IStaticTestSuite sourceTestSuite, IStaticTestSuite destinationTestSuite)
        {
            string results = string.Empty;

            foreach (ITestSuiteEntry sourceTestCaseEntry in sourceTestSuite.TestCases)
            {
                ITestCase sourceTestCase = sourceTestCaseEntry.TestCase;

                results += CopyTestCase(sourceTestCase, destinationTestSuite);
            }

            return results;
        }

        /// <summary>
        /// Copies a test case.
        /// </summary>
        /// <param name="sourceTestCase">The source test case.</param>
        /// <param name="destinationTestSuite">The destination test suite.</param>
        private static string CopyTestCase(ITestCase sourceTestCase, IStaticTestSuite destinationTestSuite)
        {
            string results = string.Empty;

            ITestCase destinationTestCase = destinationTestSuite.Project.TestCases.Create();

            destinationTestCase.Title = sourceTestCase.Title;
            destinationTestCase.Description = sourceTestCase.Description;
            destinationTestCase.Priority = sourceTestCase.Priority;


            Debug.WriteLine("Test Case: " + sourceTestCase.Title);
            foreach (Field aField in sourceTestCase.CustomFields)
            {
                Debug.WriteLine(string.Format("Field Name: '{0}', Value: '{1}'", aField.Name, aField.Value));
            }

            List<Field> trueCustomFields = (from aField in destinationTestCase.CustomFields.Cast<Field>()
                                            where !aField.ReferenceName.StartsWith("Microsoft") &&
                                            !aField.ReferenceName.StartsWith("System")
                                            select aField).ToList();

            foreach (Field destField in trueCustomFields)
            {
                Field sourceField = (from aField in sourceTestCase.CustomFields.Cast<Field>()
                                     where aField.ReferenceName == destField.ReferenceName
                                     select aField).FirstOrDefault();

                if (sourceField != null)
                {
                    destField.Value = sourceField.Value;
                }
            }

            // Set Area and Iteration Paths
            string areaPath = sourceTestCase.CustomFields["Area Path"].Value.ToString();
            if (areaPath.Contains("\\"))
            {
                areaPath = areaPath.Replace(sourceTestCase.Project.TeamProjectName, destinationTestSuite.Project.TeamProjectName);  // replace the project name

                int areaId = (from node in _destinationAreaNodes
                              where node.Path == areaPath
                              select node.Id).FirstOrDefault();


                destinationTestCase.CustomFields["Area Path"].Value = areaPath;
                destinationTestCase.CustomFields["Area ID"].Value = areaId;
            }

            string iterationPath = sourceTestCase.CustomFields["Iteration Path"].Value.ToString();
            if (iterationPath.Contains("\\"))
            {
                iterationPath = iterationPath.Replace(sourceTestCase.Project.TeamProjectName, destinationTestSuite.Project.TeamProjectName);  // replace the project name

                int iterationId = (from node in _destinationIterationNodes
                                   where node.Path == iterationPath
                                   select node.Id).FirstOrDefault();

                destinationTestCase.CustomFields["Iteration Path"].Value = iterationPath;
                destinationTestCase.CustomFields["Iteration ID"].Value = iterationId;
            }

            #region Attachments

            foreach (ITestAttachment sourceAttachment in sourceTestCase.Attachments)
            {
                string filePath = Path.Combine(Path.GetTempPath(), sourceAttachment.Name);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                sourceAttachment.DownloadToFile(filePath);

                ITestAttachment destinationAttachment = destinationTestCase.CreateAttachment(filePath);
                destinationAttachment.AttachmentType = sourceAttachment.AttachmentType;
                destinationAttachment.Comment = sourceAttachment.Comment;

                destinationTestCase.Attachments.Add(destinationAttachment);

                destinationTestCase.Save();

                File.Delete(filePath);
            }

            #endregion

            #region Test Steps/Parameters

            foreach (ITestParameter sourceParameter in sourceTestCase.TestParameters)
            {
                destinationTestCase.ReplaceParameter(sourceParameter.Name, sourceParameter.Value);
            }

            foreach (ITestStep sourceAction in sourceTestCase.Actions)
            {
                ITestStep destinationTestStep = destinationTestCase.CreateTestStep();

                destinationTestStep.Title = sourceAction.Title;
                destinationTestStep.Description = sourceAction.Description;
                destinationTestStep.TestStepType = sourceAction.TestStepType;
                destinationTestStep.ExpectedResult = sourceAction.ExpectedResult;
                destinationTestCase.Actions.Add(destinationTestStep);

                // Test Step Attachments
                foreach (ITestAttachment sourceAttachment in sourceAction.Attachments)
                {
                    string filePath = Path.Combine(Path.GetTempPath(), sourceAttachment.Name);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    sourceAttachment.DownloadToFile(filePath);

                    ITestAttachment destinationAttachment = destinationTestStep.CreateAttachment(filePath);
                    destinationAttachment.AttachmentType = sourceAttachment.AttachmentType;
                    destinationAttachment.Comment = sourceAttachment.Comment;

                    destinationTestStep.Attachments.Add(destinationAttachment);

                    try
                    {
                        destinationTestCase.Save();
                    }
                    catch (FileAttachmentException fileException)
                    {
                        destinationTestStep.Attachments.Remove(destinationAttachment);
                        results += string.Format(" - Suite: '{0}', Test Case: '{1}', Could not added attachment '{2}' due to '{3}'" + Environment.NewLine, destinationTestSuite.Title, destinationTestCase.Title, sourceAttachment.Name, fileException.Message);
                    }

                    File.Delete(filePath);
                }
            }

            #endregion

            destinationTestCase.Save();

            destinationTestCase.State = sourceTestCase.State;

            TeamFoundationIdentity sourceIdentity = sourceTestCase.Owner;

            if (sourceIdentity == null)
            {
                results += string.Format(" - Suite: '{0}', Test Case: '{1}', Could not set Assigned To user, not setup as a TFS user." + Environment.NewLine, destinationTestSuite.Title, destinationTestCase.Title);                
            }
            else
            {

                TeamFoundationIdentity destinationIdentity = null;
                try
                {
                    destinationIdentity = destinationTestCase.Project.TfsIdentityStore.FindByAccountName(sourceIdentity.UniqueName);
                }
                catch (Exception e) { }

                if (destinationIdentity != null && destinationIdentity.IsActive)
                {
                    destinationTestCase.Owner = destinationIdentity;
                }
                else
                {
                    results += string.Format(" - Suite: '{0}', Test Case: '{1}', Could not set Assigned To user to '{2}'" + Environment.NewLine, destinationTestSuite.Title, destinationTestCase.Title, sourceIdentity.UniqueName);
                }
            }

            destinationTestCase.Save();

            destinationTestSuite.Entries.Add(destinationTestCase);
            destinationTestSuite.Plan.Save();

            return results;
        }

        /// <summary>
        /// Gets the default configuration collection.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        private static IdAndName[] GetDefaultConfigurationCollection(ITestManagementTeamProject project)
        {
            ITestConfiguration defaultConfig = null;
            foreach (ITestConfiguration config in project.TestConfigurations.Query("Select * from TestConfiguration"))
            {
                defaultConfig = config;
                break;
            }

            IdAndName defaultConfigIdAndName = new IdAndName(defaultConfig.Id, defaultConfig.Name);
            return new IdAndName[] { defaultConfigIdAndName };
        }

    }
}
