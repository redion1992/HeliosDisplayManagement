﻿using System;
using System.Drawing;
using System.Linq;
using EDIDParser;
using EDIDParser.Descriptors;
using EDIDParser.Enums;
using NvAPIWrapper.Mosaic;
using NvAPIWrapper.Native.Mosaic;

namespace HeliosDisplayManagement.Shared.NVIDIA
{
    public class SurroundTopologyDisplay : IEquatable<SurroundTopologyDisplay>
    {
        public SurroundTopologyDisplay(GridTopologyDisplay display)
        {
            DisplayId = display.DisplayDevice.DisplayId;
            Rotation = display.Rotation.ToRotation();
            Overlap = new Point(display.Overlap.HorizontalOverlap, display.Overlap.VerticalOverlap);
            PixelShiftType = display.PixelShiftType;
            try
            {
                var bytes = display.DisplayDevice.PhysicalGPU.ReadEDIDData(display.DisplayDevice.Output);
                DisplayName = new EDID(bytes).Descriptors
                    .Where(descriptor => descriptor is StringDescriptor)
                    .Cast<StringDescriptor>()
                    .FirstOrDefault(descriptor => descriptor.Type == StringDescriptorType.MonitorName)?.Value;
            }
            catch
            {
                // ignored
            }
        }

        public SurroundTopologyDisplay()
        {
        }

        public uint DisplayId { get; set; }

        public string DisplayName { get; set; }

        public Point Overlap { get; set; }
        public PixelShiftType PixelShiftType { get; set; }
        public Rotation Rotation { get; set; }

        /// <inheritdoc />
        public bool Equals(SurroundTopologyDisplay other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (DisplayId == other.DisplayId) && Overlap.Equals(other.Overlap) &&
                   (PixelShiftType == other.PixelShiftType) && (Rotation == other.Rotation);
        }

        /// <inheritdoc />
        public static bool operator ==(SurroundTopologyDisplay left, SurroundTopologyDisplay right)
        {
            return Equals(left, right);
        }

        /// <inheritdoc />
        public static bool operator !=(SurroundTopologyDisplay left, SurroundTopologyDisplay right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SurroundTopologyDisplay) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) DisplayId;
                hashCode = (hashCode*397) ^ Overlap.GetHashCode();
                hashCode = (hashCode*397) ^ (int) PixelShiftType;
                hashCode = (hashCode*397) ^ (int) Rotation;
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName ?? $"SurroundTopologyDisplay #{DisplayId}";
        }

        public GridTopologyDisplay ToGridTopologyDisplay()
        {
            return new GridTopologyDisplay(DisplayId, new Overlap(Overlap.X, Overlap.Y), Rotation.ToRotate(), 0,
                PixelShiftType);
        }
    }
}