using Cybele.Collections;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UT.Cybele.Collections {
    [TestClass, TestCategory("StickyList")]
    public sealed class StickyListTests {
        [TestMethod] public void Empty() {
            // Arrange

            // Act
            var list = new StickyList<float>();

            // Assert
            list.Should().BeEmpty();
            list.LargestIndex.Should().Be(-1);
            (list as ICollection<float>).IsReadOnly.Should().BeFalse();
        }

        [TestMethod] public void ConstructFromEnumerable() {
            // Arrange
            var contents = new string[] { "Las Cruces", "St. Augustine", "Asheville", "Scranton", "Wilmington" };

            // Act
            var defaultList = new StickyList<string>(contents);
            var customList = new StickyList<string>(contents, StringComparer.OrdinalIgnoreCase);

            // Assert
            defaultList.Should().HaveCount(contents.Length);
            customList.Should().HaveCount(contents.Length);
            for (var i = 0; i < contents.Length; ++i) {
                defaultList[i].Should().Be(contents[i]);
                defaultList.IsOccupied(i).Should().BeTrue();
                defaultList.IsSticky(i).Should().BeFalse();

                customList[i].Should().Be(contents[i]);
                customList.IsOccupied(i).Should().BeTrue();
                customList.IsSticky(i).Should().BeFalse();
            }
            defaultList.HasGaps.Should().BeFalse();
            customList.HasGaps.Should().BeFalse();
        }

        [TestMethod] public void AddNonSticky() {
            // Arrange
            var list = new StickyList<int>();
            var elt0 = 49;
            var elt1 = 316722;
            var elt2 = -9281;

            // Act
            list.Add(elt0);
            list.Add(elt1);
            list.Add(elt2);

            // Assert
            list.Should().HaveCount(3);
            list[0].Should().Be(elt0);
            list[1].Should().Be(elt1);
            list[2].Should().Be(elt2);
            for (int i = 0; i <= 2; ++i) {
                list.IsOccupied(i).Should().BeTrue();
                list.IsSticky(i).Should().BeFalse();
            }
            list.HasGaps.Should().BeFalse();
        }

        [TestMethod] public void AddNonStickyToFillGaps() {
            // Arrange
            var list = new StickyList<double>();
            list.Insert(3, 525.600);
            list.Insert(0, -12.44414);
            list.HasGaps.Should().BeTrue();

            // Act
            list.Add(72.07099);
            list.Add(-23.1);
            list.Add(691827.00128);

            // Assert
            list.HasGaps.Should().BeFalse();
            list.Should().HaveCount(5);
            list.LargestIndex.Should().Be(list.Count - 1);
        }

        [TestMethod] public void InsertSticky() {
            // Arrange
            var list = new StickyList<char>();
            var elt2 = ('x', 2);
            var elt5 = ('y', 5);
            var elt119 = ('z', 119);

            // Act
            list.Insert(elt2.Item2, elt2.Item1);
            list.Insert(elt5.Item2, elt5.Item1);
            list.Insert(elt119.Item2, elt119.Item1);

            // Assert
            list.Should().HaveCount(3);
            list[elt2.Item2].Should().Be(elt2.Item1);
            list[elt5.Item2].Should().Be(elt5.Item1);
            list[elt119.Item2].Should().Be(elt119.Item1);
            for (int i = 0; i <= elt119.Item2; ++i) {
                list.IsOccupied(i).Should().Be(i == elt2.Item2 || i == elt5.Item2 || i == elt119.Item2);
                list.IsSticky(i).Should().Be(i == elt2.Item2 || i == elt5.Item2 || i == elt119.Item2);
            }
            list.HasGaps.Should().BeTrue();
        }

        [TestMethod] public void InsertStickyDisplaceNonStickyToEnd() {
            // Arrange
            var list = new StickyList<string>();
            list.Add("Harrisburg");
            list.Add("Scottsdale");
            list.Add("Bentonville");

            // Act
            list.HasGaps.Should().BeFalse();
            list.IsSticky(1).Should().BeFalse();
            list.Insert(1, "Fort Worth");

            // Assert
            list.Should().HaveCount(4);
            list.HasGaps.Should().BeFalse();
            list.IsSticky(1).Should().BeTrue();
            list.IsSticky(3).Should().BeFalse();
        }

        [TestMethod] public void InsertStickyDisplaceNonStickToGap() {
            // Arrange
            var list = new StickyList<string>();
            list.Add("Bakersfield");
            list[2] = "Lincoln";

            // Act
            list.HasGaps.Should().BeTrue();
            list.IsSticky(0).Should().BeFalse();
            list.IsOccupied(1).Should().BeFalse();
            list.Insert(0, "Santa Ana");

            // Assert
            list.Should().HaveCount(3);
            list.HasGaps.Should().BeFalse();
            list.IsSticky(0).Should().BeTrue();
            list.IsOccupied(1).Should().BeTrue();
            list.IsSticky(1).Should().BeFalse();
        }

        [TestMethod] public void InsertNegativeIndex() {
            // Arrange
            var list = new StickyList<int>();

            // Act
            Action insert = () => list.Insert(-5, 111111);
            Action set = () => list[-192] = 812;

            // Assert
            insert.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            set.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void InsertAlreadyStickiedIndex() {
            // Arrange
            var list = new StickyList<string>();
            var idx = 7;
            list.Insert(idx, "Cheyenne");

            // Act
            Action act = () => list.Insert(idx, "Newark");

            // Assert
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage($"*{idx}*");
        }

        [TestMethod] public void RemoveFromEnd() {
            // Arrange
            var list = new StickyList<DateTime>();
            list.Add(new DateTime(1984, 7, 12));
            list.Insert(8, new DateTime(2021, 7, 23));
            list.Add(new DateTime(1988, 9, 17));
            list.Insert(4, new DateTime(2000, 9, 15));
            list.Insert(5, new DateTime(2004, 8, 13));
            list.Add(new DateTime(1992, 7, 25));
            list.Add(new DateTime(1996, 7, 19));
            list.Insert(7, new DateTime(2016, 8, 5));
            list.Add(new DateTime(2012, 7, 25));

            // Act
            list.RemoveAt(list.Count - 1);
            list.Remove(new DateTime(2016, 8, 5));

            // Assert
            list.Contains(new DateTime(2016, 8, 5)).Should().BeFalse();
            list.Contains(new DateTime(2021, 7, 23)).Should().BeFalse();
            list.HasGaps.Should().BeFalse();
        }

        [TestMethod] public void RemoveCreateGaps() {
            // Arrange
            var list = new StickyList<char>();
            list.Add('*');
            list.Insert(3, '+');
            list.Add('0');
            list.Add('\'');
            list.Add('?');
            list.Add('%');

            // Act
            list.IsSticky(3).Should().BeTrue();
            list.HasGaps.Should().BeFalse();
            list.RemoveAt(3);
            list.Remove('?');

            // Assert
            list.IsSticky(3).Should().BeFalse();
            list.HasGaps.Should().BeFalse();
            list.LargestIndex.Should().Be(list.Count - 1);
        }

        [TestMethod] public void RemoveOnlyOneIfDuplicates() {
            // Arrange
            var list = new StickyList<string>();
            list.Insert(2, "Boulder");
            list.Insert(0, "Boulder");
            list.Insert(1, "Knoxville");
            list.Add("Davenport");

            // Act
            list.Contains("Boulder").Should().BeTrue();
            list.Remove("Boulder");

            // Assert
            list.Contains("Boulder").Should().BeTrue();
            list.HasGaps.Should().BeFalse();
        }

        [TestMethod] public void RemoveUntilEmpty() {
            // Arrange
            var list = new StickyList<string>();
            list.Insert(18, "Wisconsin Dells");
            list.Insert(137, "Coeur d'Alene");
            list.Insert(81, "Durham");
            list.Insert(2896, "Aurora");

            // Act
            list.Remove("Wisconsin Dells");
            list.RemoveAt(137);
            list.RemoveAt(81);
            list.Remove("Aurora");

            // Assert
            list.Should().BeEmpty();
            list.Should().HaveCount(0);
            list.LargestIndex.Should().Be(-1);
        }

        [TestMethod] public void RemoveMissingElement() {
            // Arrange
            var list = new StickyList<int>();
            list[8] = 192;
            list[2] = 192;
            list[17] = 192;

            // Act
            var result = list.Remove(2);

            // Asert
            result.Should().BeFalse();
        }

        [TestMethod] public void RemoveAtIndexOutOfBounds() {
            // Arrange
            var list = new StickyList<string>();
            list.Insert(44, "Lawrence");
            list.Add("Bridgeport");
            list.Add("Pomona");

            // Act
            Action removeNegative = () => list.RemoveAt(-681);
            Action removeTooLarge = () => list.RemoveAt(list.LargestIndex + 1);

            // Assert
            removeNegative.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            removeTooLarge.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveAtGapIndex() {
            // Arrange
            var list = new StickyList<string>();
            list.Insert(21, "Stillwater");

            // Act
            Action remove = () => list.RemoveAt(13);

            // Assert
            remove.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void Clear() {
            // Arrange
            var list = new StickyList<char>();
            list.Add('&');
            list.Insert(4, '%');
            list.Add('E');
            list.Add('=');
            list.Insert(2, '/');

            // Act
            list.Should().NotBeEmpty();
            list.Clear();

            // Assert
            list.Should().BeEmpty();
        }

        [TestMethod] public void Iteration() {
            // Arrange
            var list = new StickyList<char>() { '-', '+', '-', '+', '-', '+', '-', '+', '-', '+' };

            // Act
            var strongIter = list.GetEnumerator();
            var weakIter = (list as IEnumerable).GetEnumerator();

            // Assert
            while (strongIter.MoveNext()) {
                weakIter.MoveNext().Should().BeTrue();
                weakIter.Current.Should().Be(strongIter.Current);
            }
            weakIter.MoveNext().Should().BeFalse();
        }

        [TestMethod] public void GetItemOutOfBounds() {
            // Arrange
            var list = new StickyList<int>();
            list.Add(35);
            list.Add(-1);
            list.Add(0);

            // Act
            Func<int> actPos = () => list[25];
            Func<int> actNeg = () => list[-3];

            // Assert
            actPos.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            actNeg.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void GetItemAtGap() {
            // Arrange
            var list = new StickyList<int>();
            list.Insert(4, 405);
            var gapIdx = 3;

            // Act
            list.IsOccupied(gapIdx).Should().BeFalse();
            Func<int> act = () => list[gapIdx];

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("*gap*");
        }

        [TestMethod] public void IndexOfExistingItem() {
            // Arrange
            var list = new StickyList<char>();
            list.Add('7');
            list.Add('u');
            list.Add('_');
            list.Add('í');

            // Act
            var idx0 = list.IndexOf('7');        // the order of these is guarnated because no removals have occurred
            var idx1 = list.IndexOf('u');
            var idx2 = list.IndexOf('_');
            var idx3 = list.IndexOf('í');

            // Assert
            idx0.Should().Be(0);
            idx1.Should().Be(1);
            idx2.Should().Be(2);
            idx3.Should().Be(3);
        }

        [TestMethod] public void IndexOfExistingItemCustomComparer() {
            // Arrange
            var list = new StickyList<string>(StringComparer.OrdinalIgnoreCase);
            list.Add("Lubbock");
            list.Add("Kalamazoo");
            list.Add("Mobile");

            // Act
            var idx0 = list.IndexOf("LuBbOCK");  // the order of these is guaranteed because no removals have ocurred
            var idx1 = list.IndexOf("kalamazoo");
            var idx2 = list.IndexOf("MOBILE");

            // Assert
            idx0.Should().Be(0);
            idx1.Should().Be(1);
            idx2.Should().Be(2);
        }

        [TestMethod] public void IndexOfMissingItem() {
            // Arrange
            var list = new StickyList<double>();
            list.Add(double.MinValue);
            list.Add(double.MaxValue);
            list.Add(double.Epsilon);

            // Act
            var idx0 = list.IndexOf(38);
            var idx1 = list.IndexOf(-2.8);

            // Assert
            idx0.Should().Be(-1);
            idx1.Should().Be(-1);
        }

        [TestMethod] public void ContainsExistingItem() {
            // Arrange
            var list = new StickyList<string>();
            list.Add("South Bend");
            list.Insert(0, "Pasadena");
            list.Insert(4, "Fargo");

            // Act
            var contains0 = list.Contains(list[0]);
            var contains1 = list.Contains(list[1]);
            var contains4 = list.Contains(list[4]);

            // Assert
            contains0.Should().BeTrue();
            contains1.Should().BeTrue();
            contains4.Should().BeTrue();
        }

        [TestMethod] public void ContainsExistingItemCustomComparer() {
            // Arrange
            var list = new StickyList<string>(StringComparer.OrdinalIgnoreCase);
            list.Insert(8, "Pierre");
            list.Add("Billings");

            // Act
            var contains0 = list.Contains(list[0].ToUpper());
            var contains8 = list.Contains(list[8].ToLower());

            // Assert
            contains0.Should().BeTrue();
            contains8.Should().BeTrue();
        }

        [TestMethod] public void ContainsMissingItem() {
            // Arrange
            var list = new StickyList<string>();
            list.Insert(5, "Arlington");
            list.Insert(21, "Tuscaloosa");
            list.Add("Duluth");
            list.Insert(0, "Joliet");

            // Act
            var missing0 = list.Contains("Santa Cruz");
            var missing1 = list.Contains("Murfreesboro");

            // Assert
            missing0.Should().BeFalse();
            missing1.Should().BeFalse();
        }

        [TestMethod] public void QuerySlotsPastTheEnd() {
            // Arrange
            var list = new StickyList<int>();
            list.Add(2);
            list.Add(-110);
            list.Add(int.MaxValue);
            list.Add(int.MinValue);
            list.Add(0);

            // Act & Assert
            for (var i = list.Count; i < list.Count + 100; ++i) {
                Func<bool> isOccupied = () => list.IsOccupied(i);
                Func<bool> isSticky = () => list.IsSticky(i);

                isOccupied.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
                isSticky.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            }
        }

        [TestMethod] public void QueryNegativeIndex() {
            // Arrange
            var list = new StickyList<DateTime>();

            // Act
            Func<bool> actOccupied = () => list.IsOccupied(-44);
            Func<bool> actSticky = () => list.IsSticky(-1);

            // Assert
            actOccupied.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            actSticky.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void CopyToArray() {
            // Arrange
            var list = new StickyList<char>();
            list.Add('h');
            list.Add('~');
            list.Insert(37, '(');
            list.Insert(44, ')');
            list.Add(',');

            // Act
            char[] array = new char[10];
            (list as ICollection<char>).CopyTo(array, 3);

            // Assert
            array[3].Should().Be('h');
            array[4].Should().Be('~');
            array[5].Should().Be(',');
            array[6].Should().Be('(');
            array[7].Should().Be(')');
        }
    }
}
