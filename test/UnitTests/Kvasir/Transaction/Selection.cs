using FluentAssertions;
using Kvasir.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;

using static UT.Kvasir.Transaction.Selection;

namespace UT.Kvasir.Transaction {
    [TestClass, TestCategory("Selection")]
    public class SelectionTests {
        [TestMethod] public void ZeroInstances() {
            var fixture = new TestFixture(typeof(Samurai));

            // Act
            fixture.Transactor.SelectAll();
            var samuraiQuery = fixture.PrincipalCommands<Samurai>().SelectAllQuery;
            var samurais = fixture.Depot[typeof(Samurai)].Cast<Samurai>().ToList();

            // Assert
            samuraiQuery.Connection.Should().Be(fixture.Connection);
            samuraiQuery.Received(1).ExecuteReader();
            samurais.Should().HaveCount(0);
        }

        [TestMethod] public void SingleInstanceSingleEntityNoNullsNoRelations() {
            // Arrange
            var diaper = new object[] { Guid.NewGuid(), false, 12.5f, "Huggies", false };
            var fixture = new TestFixture(typeof(Diaper)).WithEntityRow<Diaper>(diaper);

            // Act
            fixture.Transactor.SelectAll();
            var diaperQuery = fixture.PrincipalCommands<Diaper>().SelectAllQuery;
            var diapers = fixture.Depot[typeof(Diaper)].Cast<Diaper>().ToList();

            // Assert
            diaperQuery.Connection.Should().Be(fixture.Connection);
            diaperQuery.Received(1).ExecuteReader();
            diapers.Should().HaveCount(1);
            diaper[0].Should().Be(diapers[0].DiaperID);
            diaper[1].Should().Be(diapers[0].IsUsed);
            diaper[2].Should().Be(diapers[0].Volume);
            diaper[3].Should().Be(diapers[0].Brand);
            diaper[4].Should().Be(diapers[0].ForAdults);
        }

        [TestMethod] public void SingleInstanceSingleEntityNullsNoRelations() {
            // Arrange
            var test = new object[] { "MBTI", 94U, DBNull.Value, (byte)16, false, DBNull.Value };
            var fixture = new TestFixture(typeof(PersonalityTest)).WithEntityRow<PersonalityTest>(test);

            // Act
            fixture.Transactor.SelectAll();
            var testQuery = fixture.PrincipalCommands<PersonalityTest>().SelectAllQuery;
            var tests = fixture.Depot[typeof(PersonalityTest)].Cast<PersonalityTest>().ToList();

            // Assert
            testQuery.Connection.Should().Be(fixture.Connection);
            testQuery.Received(1).ExecuteReader();
            tests.Should().HaveCount(1);
            test[0].Should().Be(tests[0].TestName);
            test[1].Should().Be(tests[0].NumQuestions);
            tests[0].AccuracyPercentage.Should().BeNull();
            test[3].Should().Be(tests[0].Discriminations);
            test[4].Should().Be(tests[0].IsAMAApproved);
            tests[0].DebutYear.Should().BeNull();
        }

        [TestMethod] public void MultipleInstancesSingleEntityNoRelations() {
            // Arrange
            var carotid = new object[] { "Carotid", (byte)2, 115U, "Internal Jugular" };
            var femoral = new object[] { "Femoral", (byte)2, 115U, "Femoral" };
            var aorta = new object[] { "Aorta", (byte)1, 110U, "Vena Cava" };
            var fixture = new TestFixture(typeof(Artery))
                .WithEntityRow<Artery>(carotid)
                .WithEntityRow<Artery>(femoral)
                .WithEntityRow<Artery>(aorta);

            // Act
            fixture.Transactor.SelectAll();
            var arteryQuery = fixture.PrincipalCommands<Artery>().SelectAllQuery;
            var arteries = fixture.Depot[typeof(Artery)].Cast<Artery>().ToList();

            // Assert
            arteryQuery.Connection.Should().Be(fixture.Connection);
            arteryQuery.Received(1).ExecuteReader();
            arteries.Should().HaveCount(3);
            carotid[0].Should().Be(arteries[0].Name);
            carotid[1].Should().Be(arteries[0].NumPerPerson);
            carotid[2].Should().Be(arteries[0].BloodFlowPSV);
            carotid[3].Should().Be(arteries[0].Vein);
            femoral[0].Should().Be(arteries[1].Name);
            femoral[1].Should().Be(arteries[1].NumPerPerson);
            femoral[2].Should().Be(arteries[1].BloodFlowPSV);
            femoral[3].Should().Be(arteries[1].Vein);
            aorta[0].Should().Be(arteries[2].Name);
            aorta[1].Should().Be(arteries[2].NumPerPerson);
            aorta[2].Should().Be(arteries[2].BloodFlowPSV);
            aorta[3].Should().Be(arteries[2].Vein);
        }

        [TestMethod] public void SingleInstanceSingleEntityNonEmptyScalarRelations() {
            // Arrange
            var table = new object[] { "PLT1TR4", (ushort)2021, true };
            var femaleRow0 = new object[] { table[0], table[1], 82, .056911 };
            var maleRow0 = new object[] { table[0], table[1], 28, .002330 };
            var maleRow1 = new object[] { table[0], table[1], 57, .011398 };
            var maleRow2 = new object[] { table[0], table[1], 119, .906532 };
            var fixture = new TestFixture(typeof(ActuarialTable))
                .WithEntityRow<ActuarialTable>(table)
                .WithRelationRow<ActuarialTable>(0, femaleRow0)
                .WithRelationRow<ActuarialTable>(1, maleRow0)
                .WithRelationRow<ActuarialTable>(1, maleRow1)
                .WithRelationRow<ActuarialTable>(1, maleRow2);

            // Act
            fixture.Transactor.SelectAll();
            var tableQuery = fixture.PrincipalCommands<ActuarialTable>().SelectAllQuery;
            var femaleQuery = fixture.RelationCommands<ActuarialTable>(0).SelectAllQuery;
            var maleQuery = fixture.RelationCommands<ActuarialTable>(1).SelectAllQuery;
            var tables = fixture.Depot[typeof(ActuarialTable)].Cast<ActuarialTable>().ToList();

            // Assert
            tableQuery.Connection.Should().Be(fixture.Connection);
            femaleQuery.Connection.Should().Be(fixture.Connection);
            maleQuery.Connection.Should().Be(fixture.Connection);
            tables.Should().HaveCount(1);
            table[0].Should().Be(tables[0].TableID);
            table[1].Should().Be(tables[0].Year);
            table[2].Should().Be(tables[0].EndorsedBySSA);
            tables[0].FemaleDeathProbability.Count.Should().Be(1);
            femaleRow0[3].Should().Be(tables[0].FemaleDeathProbability[(int)femaleRow0[2]]);
            tables[0].MaleDeathProbability.Count.Should().Be(3);
            maleRow0[3].Should().Be(tables[0].MaleDeathProbability[(int)maleRow0[2]]);
            maleRow1[3].Should().Be(tables[0].MaleDeathProbability[(int)maleRow1[2]]);
            maleRow2[3].Should().Be(tables[0].MaleDeathProbability[(int)maleRow2[2]]);
            fixture.ShouldBeOrdered(tableQuery, (femaleQuery, maleQuery));
        }

        [TestMethod] public void SingleInstanceSingleEntityEmptyScalarRelations() {
            // Arrange
            var quasar = new object[] { "3C 273", "Virgo", 0.158339, 2433000000UL };
            var fixture = new TestFixture(typeof(Quasar)).WithEntityRow<Quasar>(quasar);

            // Act
            fixture.Transactor.SelectAll();
            var quasarQuery = fixture.PrincipalCommands<Quasar>().SelectAllQuery;
            var discoverersQuery = fixture.RelationCommands<Quasar>(0).SelectAllQuery;
            var quasars = fixture.Depot[typeof(Quasar)].Cast<Quasar>().ToList();

            // Assert
            quasarQuery.Connection.Should().Be(fixture.Connection);
            discoverersQuery.Connection.Should().Be(fixture.Connection);
            quasars.Should().HaveCount(1);
            quasar[0].Should().Be(quasars[0].Designation);
            quasar[1].Should().Be(quasars[0].Constellation);
            quasar[2].Should().Be(quasars[0].Redshift);
            quasar[3].Should().Be(quasars[0].Distance);
            quasars[0].Discoverers.Count.Should().Be(0);
            fixture.ShouldBeOrdered(quasarQuery, discoverersQuery);
        }

        [TestMethod] public void MultipleInstancesSingleEntityScalarRelations() {
            // Arrange
            var oldSpice = new object[] { Guid.NewGuid(), "Old Spice", (decimal)17.99, false };
            var degreeUltra = new object[] { Guid.NewGuid(), "Degree Ultra", (decimal)24.99, false };
            var dove = new object[] { Guid.NewGuid(), "Dove", (decimal)8.75, true };
            var vanilla = new object[] { oldSpice[0], 0U, "Vanilla" };
            var cinnamon = new object[] { oldSpice[0], 1U, "Cinnamon" };
            var peppermint = new object[] { degreeUltra[0], 0U, "Peppermint" };
            var lemongrass = new object[] { dove[0], 0U, "Lemongrass" };
            var coconut = new object[] { dove[0], 1U, "Coconut" };
            var mango = new object[] { dove[0], 2U, "Mango" };
            var fixture = new TestFixture(typeof(Deodorant))
                .WithEntityRow<Deodorant>(oldSpice)
                .WithEntityRow<Deodorant>(degreeUltra)
                .WithEntityRow<Deodorant>(dove)
                .WithRelationRow<Deodorant>(0, vanilla)
                .WithRelationRow<Deodorant>(0, cinnamon)
                .WithRelationRow<Deodorant>(0, peppermint)
                .WithRelationRow<Deodorant>(0, lemongrass)
                .WithRelationRow<Deodorant>(0, coconut)
                .WithRelationRow<Deodorant>(0, mango);

            // Act
            fixture.Transactor.SelectAll();
            var deodorantQuery = fixture.PrincipalCommands<Deodorant>().SelectAllQuery;
            var scentsQuery = fixture.RelationCommands<Deodorant>(0).SelectAllQuery;
            var deodorants = fixture.Depot[typeof(Deodorant)].Cast<Deodorant>().ToList();

            // Assert
            deodorantQuery.Connection.Should().Be(fixture.Connection);
            scentsQuery.Connection.Should().Be(fixture.Connection);
            deodorants.Should().HaveCount(3);
            oldSpice[0].Should().Be(deodorants[0].ProductID);
            oldSpice[1].Should().Be(deodorants[0].Brand);
            oldSpice[2].Should().Be(deodorants[0].Price);
            oldSpice[3].Should().Be(deodorants[0].Antipersperant);
            deodorants[0].Scents.Count.Should().Be(2);
            vanilla[2].Should().Be(deodorants[0].Scents[0]);
            cinnamon[2].Should().Be(deodorants[0].Scents[1]);
            degreeUltra[0].Should().Be(deodorants[1].ProductID);
            degreeUltra[1].Should().Be(deodorants[1].Brand);
            degreeUltra[2].Should().Be(deodorants[1].Price);
            degreeUltra[3].Should().Be(deodorants[1].Antipersperant);
            deodorants[1].Scents.Count.Should().Be(1);
            peppermint[2].Should().Be(deodorants[1].Scents[0]);
            dove[0].Should().Be(deodorants[2].ProductID);
            dove[1].Should().Be(deodorants[2].Brand);
            dove[2].Should().Be(deodorants[2].Price);
            dove[3].Should().Be(deodorants[2].Antipersperant);
            deodorants[2].Scents.Count.Should().Be(3);
            lemongrass[2].Should().Be(deodorants[2].Scents[0]);
            coconut[2].Should().Be(deodorants[2].Scents[1]);
            mango[2].Should().Be(deodorants[2].Scents[2]);
            fixture.ShouldBeOrdered(deodorantQuery, scentsQuery);
        }

        [TestMethod] public void MultipleUnrelatedEntities() {
            // Arrange
            var griselda = new object[] { (byte)3, (byte)2, "Griselda Blanco", "New Jersey", "Dan Harmon", new DateTime(2015, 9, 8) };
            var cooper = new object[] { (byte)5, (byte)7, "D.B. Cooper", "Drunk Mystery", "Kyle Mooney", new DateTime(2018, 3, 6) };
            var lrNine = new object[] { (byte)6, (byte)4, "Little Rock Nine", "Trailblazers", "Amber Ruffin", new DateTime(2019, 2, 5) };
            var coke = new object[] { (byte)1, (byte)3, "Coca-Cola", "Atlanta", "Jenny Slate", new DateTime(2013, 7, 23) };
            var chromium = new object[] { Guid.NewGuid(), "Other", "Chromium", false, DBNull.Value };
            var shellfish = new object[] { Guid.NewGuid(), "FoodBorne", "Shellfish", true, 14.892 };
            var colonoscopy = new object[] { "Eric T. Needlebaum", new DateTime(2012, 9, 19), "Dr. Smith Andersonson", true, (sbyte)9, false };
            var fixture = new TestFixture(typeof(DrunkHistory), typeof(Allergen), typeof(Colonscopy))
                .WithEntityRow<DrunkHistory>(griselda)
                .WithEntityRow<DrunkHistory>(cooper)
                .WithEntityRow<DrunkHistory>(lrNine)
                .WithEntityRow<DrunkHistory>(coke)
                .WithEntityRow<Allergen>(chromium)
                .WithEntityRow<Allergen>(shellfish)
                .WithEntityRow<Colonscopy>(colonoscopy);

            // Act
            fixture.Transactor.SelectAll();
            var drunkHistoryQuery = fixture.PrincipalCommands<DrunkHistory>().SelectAllQuery;
            var allergenQuery = fixture.PrincipalCommands<Allergen>().SelectAllQuery;
            var colonoscopyQuery = fixture.PrincipalCommands<Colonscopy>().SelectAllQuery;
            var drunkHistories = fixture.Depot[typeof(DrunkHistory)].Cast<DrunkHistory>().ToList();
            var allergens = fixture.Depot[typeof(Allergen)].Cast<Allergen>().ToList();
            var colonoscopies = fixture.Depot[typeof(Colonscopy)].Cast<Colonscopy>().ToList();

            // Assert
            drunkHistoryQuery.Connection.Should().Be(fixture.Connection);
            allergenQuery.Connection.Should().Be(fixture.Connection);
            colonoscopyQuery.Connection.Should().Be(fixture.Connection);
            drunkHistories.Should().HaveCount(4);
            griselda[0].Should().Be(drunkHistories[0].Season);
            griselda[1].Should().Be(drunkHistories[0].EpisodeNumber);
            griselda[2].Should().Be(drunkHistories[0].Segment);
            griselda[3].Should().Be(drunkHistories[0].Title);
            griselda[4].Should().Be(drunkHistories[0].Narrator);
            griselda[5].Should().Be(drunkHistories[0].AirDate);
            cooper[0].Should().Be(drunkHistories[1].Season);
            cooper[1].Should().Be(drunkHistories[1].EpisodeNumber);
            cooper[2].Should().Be(drunkHistories[1].Segment);
            cooper[3].Should().Be(drunkHistories[1].Title);
            cooper[4].Should().Be(drunkHistories[1].Narrator);
            cooper[5].Should().Be(drunkHistories[1].AirDate);
            lrNine[0].Should().Be(drunkHistories[2].Season);
            lrNine[1].Should().Be(drunkHistories[2].EpisodeNumber);
            lrNine[2].Should().Be(drunkHistories[2].Segment);
            lrNine[3].Should().Be(drunkHistories[2].Title);
            lrNine[4].Should().Be(drunkHistories[2].Narrator);
            lrNine[5].Should().Be(drunkHistories[2].AirDate);
            coke[0].Should().Be(drunkHistories[3].Season);
            coke[1].Should().Be(drunkHistories[3].EpisodeNumber);
            coke[2].Should().Be(drunkHistories[3].Segment);
            coke[3].Should().Be(drunkHistories[3].Title);
            coke[4].Should().Be(drunkHistories[3].Narrator);
            coke[5].Should().Be(drunkHistories[3].AirDate);
            allergens.Should().HaveCount(2);
            chromium[0].Should().Be(allergens[0].AllergenID);
            chromium[1].Should().Be(ConversionOf(allergens[0].Category));
            chromium[2].Should().Be(allergens[0].Name);
            chromium[3].Should().Be(allergens[0].FDARecognized);
            allergens[0].Prevalence.Should().BeNull();
            shellfish[0].Should().Be(allergens[1].AllergenID);
            shellfish[1].Should().Be(ConversionOf(allergens[1].Category));
            shellfish[2].Should().Be(allergens[1].Name);
            shellfish[3].Should().Be(allergens[1].FDARecognized);
            shellfish[4].Should().Be(allergens[1].Prevalence);
            colonoscopies.Should().HaveCount(1);
            colonoscopy[0].Should().Be(colonoscopies[0].Patient);
            colonoscopy[1].Should().Be(colonoscopies[0].Date);
            colonoscopy[2].Should().Be(colonoscopies[0].Doctor);
            colonoscopy[3].Should().Be(colonoscopies[0].IsPreventative);
            colonoscopy[4].Should().Be(colonoscopies[0].Discomfort);
            colonoscopy[5].Should().Be(colonoscopies[0].Biopsy);
            fixture.ShouldBeOrdered((drunkHistoryQuery, allergenQuery, colonoscopyQuery));
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceChain() {
            // Arrange
            var male = new object[] { 'M', "male", 47.259 };
            var female = new object[] { 'F', "female", 46.8114 };
            var john = new object[] { "337-81-9045", "John", "Edelsburgg", male[0] };
            var nicholas = new object[] { "144-06-2252", "Nicholas", "Saint-Savez", male[0] };
            var esther = new object[] { "595-77-6211", "Esther", "Shwi", female[0] };
            var bank = new object[] { "Bank of Biscuits", '$', nicholas[0], (decimal)28500000, (decimal)8000000000 };
            var insurance = new object[] { "Goliath Insurance", 'i', esther[0], (decimal)23000000000, (decimal)400000000000 };
            var annuity = new object[] { Guid.NewGuid(), "Annuitization", (decimal)750000, john[0], insurance[0], insurance[1], true };
            var fixture = new TestFixture(typeof(Annuity), typeof(Annuity.Company), typeof(Annuity.Person), typeof(Annuity.Gender))
                .WithEntityRow<Annuity>(annuity)
                .WithEntityRow<Annuity.Company>(bank)
                .WithEntityRow<Annuity.Company>(insurance)
                .WithEntityRow<Annuity.Person>(john)
                .WithEntityRow<Annuity.Person>(nicholas)
                .WithEntityRow<Annuity.Person>(esther)
                .WithEntityRow<Annuity.Gender>(male)
                .WithEntityRow<Annuity.Gender>(female);

            // Act
            fixture.Transactor.SelectAll();
            var genderQuery = fixture.PrincipalCommands<Annuity.Gender>().SelectAllQuery;
            var personQuery = fixture.PrincipalCommands<Annuity.Person>().SelectAllQuery;
            var companyQuery = fixture.PrincipalCommands<Annuity.Company>().SelectAllQuery;
            var annuityQuery = fixture.PrincipalCommands<Annuity>().SelectAllQuery;
            var genders = fixture.Depot[typeof(Annuity.Gender)].Cast<Annuity.Gender>().ToList();
            var persons = fixture.Depot[typeof(Annuity.Person)].Cast<Annuity.Person>().ToList();
            var companies = fixture.Depot[typeof(Annuity.Company)].Cast<Annuity.Company>().ToList();
            var annuities = fixture.Depot[typeof(Annuity)].Cast<Annuity>().ToList();

            // Assert
            genderQuery.Connection.Should().Be(fixture.Connection);
            personQuery.Connection.Should().Be(fixture.Connection);
            companyQuery.Connection.Should().Be(fixture.Connection);
            annuityQuery.Connection.Should().Be(fixture.Connection);
            genders.Should().HaveCount(2);
            male[0].Should().Be(genders[0].Symbol);
            male[1].Should().Be(genders[0].Designation);
            male[2].Should().Be(genders[0].Prevalence);
            female[0].Should().Be(genders[1].Symbol);
            female[1].Should().Be(genders[1].Designation);
            female[2].Should().Be(genders[1].Prevalence);
            persons.Should().HaveCount(3);
            john[0].Should().Be(persons[0].SSN);
            john[1].Should().Be(persons[0].FirstName);
            john[2].Should().Be(persons[0].LastName);
            persons[0].Gender.Should().Be(genders[0]);
            nicholas[0].Should().Be(persons[1].SSN);
            nicholas[1].Should().Be(persons[1].FirstName);
            nicholas[2].Should().Be(persons[1].LastName);
            persons[1].Gender.Should().Be(genders[0]);
            esther[0].Should().Be(persons[2].SSN);
            esther[1].Should().Be(persons[2].FirstName);
            esther[2].Should().Be(persons[2].LastName);
            persons[2].Gender.Should().Be(genders[1]);
            companies.Should().HaveCount(2);
            bank[0].Should().Be(companies[0].CompanyName);
            bank[1].Should().Be(companies[0].Classification);
            companies[0].CEO.Should().Be(persons[1]);
            bank[3].Should().Be(companies[0].Revenue);
            bank[4].Should().Be(companies[0].MarketCap);
            insurance[0].Should().Be(companies[1].CompanyName);
            insurance[1].Should().Be(companies[1].Classification);
            companies[1].CEO.Should().Be(persons[2]);
            insurance[3].Should().Be(companies[1].Revenue);
            insurance[4].Should().Be(companies[1].MarketCap);
            annuities.Should().HaveCount(1);
            annuity[0].Should().Be(annuities[0].ID);
            annuity[1].Should().Be(ConversionOf(annuities[0].Phase));
            annuity[2].Should().Be(annuities[0].MarketValue);
            annuities[0].Annuitant.Should().Be(persons[0]);
            annuities[0].Guarantor.Should().Be(companies[1]);
            annuity[6].Should().Be(annuities[0].IsVariable);
            fixture.ShouldBeOrdered(genderQuery, personQuery, companyQuery, annuityQuery);
        }

        [TestMethod] public void MultipleEntitiesRelatedByReferenceTree() {
            // Arrange
            var elvis = new object[] { Guid.NewGuid(), "Elvis", "Presley" };
            var taylor = new object[] { Guid.NewGuid(), "Taylor", "Swift" };
            var harvard = new object[] { Guid.NewGuid(), "Harvard University", 21613UL, (decimal)50700000000 };
            var badBlood = new object[] { "Bad Blood", (short)211, taylor[0], true };
            var group = new object[] { "The Klippenkloppz", DBNull.Value, badBlood[0], 3, 11, 5, false };
            var fixture = new TestFixture(typeof(ACapellaGroup), typeof(ACapellaGroup.University), typeof(ACapellaGroup.Song), typeof(ACapellaGroup.Songwriter))
                .WithEntityRow<ACapellaGroup.Songwriter>(elvis)
                .WithEntityRow<ACapellaGroup.Songwriter>(taylor)
                .WithEntityRow<ACapellaGroup.University>(harvard)
                .WithEntityRow<ACapellaGroup.Song>(badBlood)
                .WithEntityRow<ACapellaGroup>(group);

            // Act
            fixture.Transactor.SelectAll();
            var songwriterQuery = fixture.PrincipalCommands<ACapellaGroup.Songwriter>().SelectAllQuery;
            var universityQuery = fixture.PrincipalCommands<ACapellaGroup.University>().SelectAllQuery;
            var songQuery = fixture.PrincipalCommands<ACapellaGroup.Song>().SelectAllQuery;
            var groupQuery = fixture.PrincipalCommands<ACapellaGroup>().SelectAllQuery;
            var songwriters = fixture.Depot[typeof(ACapellaGroup.Songwriter)].Cast<ACapellaGroup.Songwriter>().ToList();
            var universities = fixture.Depot[typeof(ACapellaGroup.University)].Cast<ACapellaGroup.University>().ToList();
            var songs = fixture.Depot[typeof(ACapellaGroup.Song)].Cast<ACapellaGroup.Song>().ToList();
            var groups = fixture.Depot[typeof(ACapellaGroup)].Cast<ACapellaGroup>().ToList();

            // Assert
            songwriterQuery.Connection.Should().Be(fixture.Connection);
            universityQuery.Connection.Should().Be(fixture.Connection);
            songQuery.Connection.Should().Be(fixture.Connection);
            groupQuery.Connection.Should().Be(fixture.Connection);
            songwriters.Should().HaveCount(2);
            elvis[0].Should().Be(songwriters[0].ID);
            elvis[1].Should().Be(songwriters[0].FirtName);
            elvis[2].Should().Be(songwriters[0].LastName);
            taylor[0].Should().Be(songwriters[1].ID);
            taylor[1].Should().Be(songwriters[1].FirtName);
            taylor[2].Should().Be(songwriters[1].LastName);
            universities.Should().HaveCount(1);
            harvard[0].Should().Be(universities[0].InternationalSchoolIdentifier);
            harvard[1].Should().Be(universities[0].Name);
            harvard[2].Should().Be(universities[0].Enrollment);
            harvard[3].Should().Be(universities[0].Endowment);
            songs.Should().HaveCount(1);
            badBlood[0].Should().Be(songs[0].Title);
            badBlood[1].Should().Be(songs[0].SecondsLong);
            songs[0].Writer.Should().Be(songwriters[1]);
            badBlood[3].Should().Be(songs[0].ContainsRap);
            groups.Should().HaveCount(1);
            group[0].Should().Be(groups[0].GroupName);
            groups[0].College.Should().BeNull();
            groups[0].EncoreSong.Should().Be(songs[0]);
            group[3].Should().Be(groups[0].NumAltos);
            group[4].Should().Be(groups[0].NumSopranos);
            group[5].Should().Be(groups[0].NumBaritones);
            group[6].Should().Be(groups[0].IsCoed);
            fixture.ShouldBeOrdered(songwriterQuery, songQuery, groupQuery);
            fixture.ShouldBeOrdered(universityQuery, groupQuery);
        }

        [TestMethod] public void MultipleEntitiesRelatedByRelation() {
            // Arrange
            var aaron = new object[] { "GB", 12, "Aaron Rodgers", "QB" };
            var richard = new object[] { "GB", 82, "Richard Rodgers", "TE" };
            var miracle = new object[] { new DateTime(2015, 12, 3), 63U, "DET", true };
            var fixture = new TestFixture(typeof(HailMary), typeof(HailMary.FootballPlayer))
                .WithEntityRow<HailMary>(miracle)
                .WithEntityRow<HailMary.FootballPlayer>(aaron)
                .WithEntityRow<HailMary.FootballPlayer>(richard)
                .WithRelationRow<HailMary>(0, new object[] { miracle[0], miracle[1], aaron[0], aaron[1] })
                .WithRelationRow<HailMary>(0, new object[] { miracle[0], miracle[1], richard[0], richard[1] });

            // Act
            fixture.Transactor.SelectAll();
            var hailMaryQuery = fixture.PrincipalCommands<HailMary>().SelectAllQuery;
            var playerQuery = fixture.PrincipalCommands<HailMary.FootballPlayer>().SelectAllQuery;
            var involvementQuery = fixture.RelationCommands<HailMary>(0).SelectAllQuery;
            var hailMarys = fixture.Depot[typeof(HailMary)].Cast<HailMary>().ToList();
            var players = fixture.Depot[typeof(HailMary.FootballPlayer)].Cast<HailMary.FootballPlayer>().ToList();

            // Assert
            hailMaryQuery.Connection.Should().Be(fixture.Connection);
            playerQuery.Connection.Should().Be(fixture.Connection);
            involvementQuery.Connection.Should().Be(fixture.Connection);
            hailMarys.Should().HaveCount(1);
            miracle[0].Should().Be(hailMarys[0].Date);
            miracle[1].Should().Be(hailMarys[0].PlayNumber);
            miracle[2].Should().Be(hailMarys[0].Opponent);
            miracle[3].Should().Be(hailMarys[0].ResultedInTouchdown);
            hailMarys[0].PlayersInvolved.Count.Should().Be(2);
            hailMarys[0].PlayersInvolved[0].Should().Be(players[0]);
            hailMarys[0].PlayersInvolved[1].Should().Be(players[1]);
            players.Should().HaveCount(2);
            aaron[0].Should().Be(players[0].Team);
            aaron[1].Should().Be(players[0].JerseyNumber);
            aaron[2].Should().Be(players[0].Name);
            aaron[3].Should().Be(players[0].Position);
            richard[0].Should().Be(players[1].Team);
            richard[1].Should().Be(players[1].JerseyNumber);
            richard[2].Should().Be(players[1].Name);
            richard[3].Should().Be(players[1].Position);
            fixture.ShouldBeOrdered((playerQuery, hailMaryQuery), involvementQuery);
        }

        [TestMethod] public void SelfReferentialRelation() {
            // Arrange
            var naderShah = new object[] { "Nader Shah", new DateTime(1736, 3, 8), new DateTime(1747, 6, 20), "Afsharid", "Tehran" };
            var abbas = new object[] { "Abbas III", new DateTime(1732, 4, 16), new DateTime(1736, 1, 22), "Safavid", "Tehran" };
            var tahmasp = new object[] { "Tahmasp II", new DateTime(1722, 11, 10), new DateTime(1732, 4, 16), "Safavid", "Tehran" };
            var fixture = new TestFixture(typeof(IranianShah))
                .WithEntityRow<IranianShah>(naderShah)
                .WithEntityRow<IranianShah>(abbas)
                .WithEntityRow<IranianShah>(tahmasp)
                .WithRelationRow<IranianShah>(0, new object[] { naderShah[0], abbas[0] })
                .WithRelationRow<IranianShah>(0, new object[] { abbas[0], tahmasp[0] });

            // Act
            fixture.Transactor.SelectAll();
            var shahQuery = fixture.PrincipalCommands<IranianShah>().SelectAllQuery;
            var predecessorsQuery = fixture.RelationCommands<IranianShah>(0).SelectAllQuery;
            var shahs = fixture.Depot[typeof(IranianShah)].Cast<IranianShah>().ToList();

            // Assert
            shahQuery.Connection.Should().Be(fixture.Connection);
            predecessorsQuery.Connection.Should().Be(fixture.Connection);
            shahs.Should().HaveCount(3);
            naderShah[0].Should().Be(shahs[0].Name);
            naderShah[1].Should().Be(shahs[0].ReignStart);
            naderShah[2].Should().Be(shahs[0].ReignEnd);
            naderShah[3].Should().Be(shahs[0].RoyalHouse);
            naderShah[4].Should().Be(shahs[0].Capital);
            shahs[0].Predecessor.Count.Should().Be(1);
            shahs[0].Predecessor.First().Should().Be(shahs[1]);
            abbas[0].Should().Be(shahs[1].Name);
            abbas[1].Should().Be(shahs[1].ReignStart);
            abbas[2].Should().Be(shahs[1].ReignEnd);
            abbas[3].Should().Be(shahs[1].RoyalHouse);
            abbas[4].Should().Be(shahs[1].Capital);
            shahs[1].Predecessor.Count.Should().Be(1);
            shahs[1].Predecessor.First().Should().Be(shahs[2]);
            tahmasp[0].Should().Be(shahs[2].Name);
            tahmasp[1].Should().Be(shahs[2].ReignStart);
            tahmasp[2].Should().Be(shahs[2].ReignEnd);
            tahmasp[3].Should().Be(shahs[2].RoyalHouse);
            tahmasp[4].Should().Be(shahs[2].Capital);
            shahs[2].Predecessor.Count.Should().Be(0);
            fixture.ShouldBeOrdered(shahQuery, predecessorsQuery);
        }


        private static string ConversionOf<T>(T enumerator) where T : Enum {
            var converter = new EnumToStringConverter(typeof(T)).ConverterImpl;
            return (string)converter.Convert(enumerator)!;
        }
    }
}
