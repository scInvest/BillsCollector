using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace WebClient.ViewModels
{
    public interface IRefreshableComponent
    {
        void Refresh();
    }

    // Minimal base ViewModel.
    // - INotifyPropertyChanged
    // - Parent / Children
    // - HasChanges flag
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private readonly List<ViewModelBase> _children;
        // Update batching fields
        private int _updateNesting;
        private bool _refreshPending;
        private Propagate _pendingPropagate;

        [Flags]
        public enum Propagate
        {
            None = 0,
            This = 1,
            Children = 2,
            Parents = 4,
            Root = 8,
            WholeTree = 16
        }

        private readonly Func<ComponentBase> _getComponent;

        public ViewModelBase(Func<ComponentBase> getComponent, IEnumerable<ViewModelBase>? children = null, ViewModelBase? parent = null)
        {
            if (getComponent == null)
            {
                throw new ArgumentNullException(nameof(getComponent));
            }
            _getComponent = getComponent;
            _children = children != null ? new List<ViewModelBase>(children) : new List<ViewModelBase>();
            foreach (var child in _children)
            {
                child.Parent = this;
            }
            if (parent != null)
            {
                Parent = parent;
            }
        }

        public ViewModelBase(Func<ComponentBase> getComponent, IEnumerable<ViewModelBase> children)
            : this(getComponent, children, null) { }

        public ViewModelBase(Func<ComponentBase> getComponent, ViewModelBase parent)
            : this(getComponent, null, parent) { }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModelBase? Parent { get; set; }

        public IReadOnlyList<ViewModelBase> Children => _children;

        public ComponentBase Component => _getComponent();

        public void BeginUpdate()
        {
            _updateNesting++;
        }

        public void EndUpdate()
        {
            if (_updateNesting == 0) { return; }
            _updateNesting--;
            if (_updateNesting == 0 && _refreshPending)
            {
                _refreshPending = false;
                var propagate = _pendingPropagate;
                _pendingPropagate = Propagate.None;
                RefreshInternal(propagate);
            }
        }

        public void Refresh()
        {
            Refresh(Propagate.This);
        }

        private void RefreshInternal(Propagate propagate)
        {
            if (_updateNesting > 0)
            {
                _refreshPending = true;
                _pendingPropagate |= propagate;
                return;
            }

            if ((propagate & Propagate.WholeTree) != 0)
            {
                var rootNode = this;
                while (rootNode.Parent != null)
                {
                    rootNode = rootNode.Parent;
                }
                rootNode.RefreshInternal(Propagate.This | Propagate.Children);
            }

            if ((propagate & Propagate.Root) != 0)
            {
                var root = this;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                root.RefreshInternal(Propagate.This);
            }

            if ((propagate & Propagate.This) != 0)
            {
                if(Component == null)
                {
                    throw new InvalidOperationException("Component is null.");
                }
                if (Component is IRefreshableComponent refreshable)
                {
                    refreshable.Refresh();
                }
                else
                {
                    throw new InvalidOperationException($"Component does not implement IRefreshableComponent: {Component.GetType().FullName}");
                }
            }

            if ((propagate & Propagate.Children) != 0)
            {
                foreach (var child in Children)
                {
                    child.RefreshInternal(Propagate.This | Propagate.Children);
                }
            }

            if ((propagate & Propagate.Parents) != 0)
            {
                Parent?.RefreshInternal(Propagate.This | Propagate.Parents);
            }
        }

        public void Refresh(Propagate propagate)
        {
            RefreshInternal(propagate);
        }

        private void OnPropertyChangedTrigger(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChangedTrigger(propertyName ?? string.Empty);
        }

        public void Dispose()
        {

        }
    }
}
