// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.1.0.0
//      SpecFlow Generator Version:3.1.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Rankings.IntegrationTests
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class RankingFeature : Xunit.IClassFixture<RankingFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Ranking.feature"
#line hidden
        
        public RankingFeature(RankingFeature.FixtureData fixtureData, Rankings_IntegrationTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Ranking", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 3
#line hidden
#line 4
 testRunner.Given("a player named Michel", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 5
 testRunner.And("a player named Geale", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 6
 testRunner.And("a venue named Amsterdam", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 7
 testRunner.And("a game type named tafeltennis", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 8
 testRunner.And("the current user is Michel with role Admin", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 9
 testRunner.And("elo system with k-factor 50 and n is 400 and initial elo is 1200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 10
 testRunner.And("margin of victory active", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="No games played yet")]
        [Xunit.TraitAttribute("FeatureTitle", "Ranking")]
        [Xunit.TraitAttribute("Description", "No games played yet")]
        public virtual void NoGamesPlayedYet()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("No games played yet", null, ((string[])(null)));
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 13
 testRunner.Given("no games played", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 14
 testRunner.When("I view the tafeltennis ranking", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "Ranking",
                            "NamePlayer",
                            "Points"});
                table10.AddRow(new string[] {
                            "1",
                            "Geale",
                            "1200"});
                table10.AddRow(new string[] {
                            "2",
                            "Michel",
                            "1200"});
#line 15
 testRunner.Then("we have the following tafeltennis ranking with precision 0:", ((string)(null)), table10, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="First ever game played")]
        [Xunit.TraitAttribute("FeatureTitle", "Ranking")]
        [Xunit.TraitAttribute("Description", "First ever game played")]
        public virtual void FirstEverGamePlayed()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("First ever game played", null, ((string[])(null)));
#line 20
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 21
 testRunner.Given("no games played", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "Registration Date",
                            "First Player",
                            "Second Player",
                            "S1",
                            "S2"});
                table11.AddRow(new string[] {
                            "2019-11-23 16:04",
                            "Michel",
                            "Geale",
                            "2",
                            "1"});
#line 22
 testRunner.When("the following tafeltennis games are played in Amsterdam:", ((string)(null)), table11, "When ");
#line hidden
                TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                            "Ranking",
                            "NamePlayer",
                            "Points"});
                table12.AddRow(new string[] {
                            "1",
                            "Michel",
                            "1217"});
                table12.AddRow(new string[] {
                            "2",
                            "Geale",
                            "1183"});
#line 25
 testRunner.Then("we have the following tafeltennis ranking with precision 0:", ((string)(null)), table12, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Player played two games")]
        [Xunit.TraitAttribute("FeatureTitle", "Ranking")]
        [Xunit.TraitAttribute("Description", "Player played two games")]
        public virtual void PlayerPlayedTwoGames()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Player played two games", null, ((string[])(null)));
#line 30
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 31
 testRunner.Given("no games played", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                            "Registration Date",
                            "First Player",
                            "Second Player",
                            "S1",
                            "S2"});
                table13.AddRow(new string[] {
                            "2019-11-23 16:04",
                            "Michel",
                            "Geale",
                            "2",
                            "1"});
                table13.AddRow(new string[] {
                            "2019-11-23 17:04",
                            "Michel",
                            "Geale",
                            "1",
                            "2"});
#line 32
 testRunner.When("the following tafeltennis games are played in Amsterdam:", ((string)(null)), table13, "When ");
#line hidden
                TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                            "Ranking",
                            "NamePlayer",
                            "Points"});
                table14.AddRow(new string[] {
                            "1",
                            "Geale",
                            "1202"});
                table14.AddRow(new string[] {
                            "2",
                            "Michel",
                            "1198"});
#line 36
 testRunner.Then("we have the following tafeltennis ranking with precision 0:", ((string)(null)), table14, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Two games played between the same players but registered by different players")]
        [Xunit.TraitAttribute("FeatureTitle", "Ranking")]
        [Xunit.TraitAttribute("Description", "Two games played between the same players but registered by different players")]
        public virtual void TwoGamesPlayedBetweenTheSamePlayersButRegisteredByDifferentPlayers()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two games played between the same players but registered by different players", null, ((string[])(null)));
#line 41
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 42
 testRunner.Given("no games played", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                            "Registration Date",
                            "First Player",
                            "Second Player",
                            "S1",
                            "S2"});
                table15.AddRow(new string[] {
                            "2019-11-23 16:04",
                            "Michel",
                            "Geale",
                            "2",
                            "1"});
                table15.AddRow(new string[] {
                            "2019-11-23 17:04",
                            "Geale",
                            "Michel",
                            "2",
                            "1"});
#line 43
 testRunner.When("the following tafeltennis games are played in Amsterdam:", ((string)(null)), table15, "When ");
#line hidden
                TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                            "Ranking",
                            "NamePlayer",
                            "Points"});
                table16.AddRow(new string[] {
                            "1",
                            "Geale",
                            "1202"});
                table16.AddRow(new string[] {
                            "2",
                            "Michel",
                            "1198"});
#line 47
 testRunner.Then("we have the following tafeltennis ranking with precision 0:", ((string)(null)), table16, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                RankingFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                RankingFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion