using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Company.Game.Discovery
{
    [DisallowMultipleComponent]
    public sealed class DiscoveryPanelView : MonoBehaviour
    {
        [SerializeField]
        private DiscoveryLogService discoveryLogService;

        [SerializeField]
        private Text headerLabel;

        [SerializeField]
        private Text bodyLabel;

        private void OnEnable()
        {
            if (discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged += HandleDiscoveryChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged -= HandleDiscoveryChanged;
            }
        }

        public void Configure(DiscoveryLogService service)
        {
            if (discoveryLogService == service)
            {
                return;
            }

            if (discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged -= HandleDiscoveryChanged;
            }

            discoveryLogService = service;

            if (isActiveAndEnabled && discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged += HandleDiscoveryChanged;
            }

            Refresh();
        }

        private void HandleDiscoveryChanged(IReadOnlyCollection<string> _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (bodyLabel == null)
            {
                return;
            }

            if (discoveryLogService == null)
            {
                bodyLabel.text = "No discovery log.";
                if (headerLabel != null)
                {
                    headerLabel.text = "Discovery";
                }

                return;
            }

            IReadOnlyCollection<string> discovered = discoveryLogService.PersistentDiscoveries;
            if (discovered.Count == 0)
            {
                bodyLabel.text = "Discover recipes by merging new combos.";
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (string entry in discovered)
                {
                    builder.AppendLine(entry);
                }

                bodyLabel.text = builder.ToString();
            }

            if (headerLabel != null)
            {
                headerLabel.text = $"Discovery ({discovered.Count})";
            }
        }
    }
}
