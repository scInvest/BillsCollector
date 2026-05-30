using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.ViewModels.Tests
{
    [TestClass]
    public class ViewModelBaseTests
    {
        private class TestComponent : ComponentBase, IRefreshableComponent
        {
            public int RefreshCount { get; private set; }
            public void Refresh() { RefreshCount++; }
        }

        private class TestViewModel : ViewModelBase
        {
            public TestViewModel(ComponentBase component) : base(component) { }
            public TestViewModel(ComponentBase component, IEnumerable<ViewModelBase> children, ViewModelBase parent = null) : base(component, children, parent) { }
        }

        [TestMethod]
        public void Refresh_This_OnlyThisComponentRefreshed()
        {
            var comp = new TestComponent();
            var vm = new TestViewModel(comp);
            vm.Refresh(ViewModelBase.Propagate.This);
            Assert.AreEqual(1, comp.RefreshCount);
        }

        [TestMethod]
        public void Refresh_Children_OnlyChildrenRefreshed()
        {
            var parentComp = new TestComponent();
            var childComp = new TestComponent();
            var parent = new TestViewModel(parentComp);
            var child = new TestViewModel(childComp);
            parent = new TestViewModel(parentComp, new[] { child });
            parent.Refresh(ViewModelBase.Propagate.Children);
            Assert.AreEqual(0, parentComp.RefreshCount);
            Assert.AreEqual(1, childComp.RefreshCount);
        }

        [TestMethod]
        public void Refresh_Parents_OnlyParentsRefreshed()
        {
            var parentComp = new TestComponent();
            var childComp = new TestComponent();
            var parent = new TestViewModel(parentComp);
            var child = new TestViewModel(childComp, null, parent);
            child.Refresh(ViewModelBase.Propagate.Parents);
            Assert.AreEqual(1, parentComp.RefreshCount);
            Assert.AreEqual(0, childComp.RefreshCount);
        }

        [TestMethod]
        public void Refresh_WholeTree_AllRefreshed()
        {
            var rootComp = new TestComponent();
            var childComp = new TestComponent();
            var root = new TestViewModel(rootComp);
            var child = new TestViewModel(childComp, null, root);
            root = new TestViewModel(rootComp, new[] { child });
            child.Refresh(ViewModelBase.Propagate.WholeTree);
            Assert.AreEqual(1, rootComp.RefreshCount);
            Assert.AreEqual(1, childComp.RefreshCount);
        }
    }
}
