﻿#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Text.RegularExpressions;
using Mediaportal.TV.Server.TVDatabase.Entities.Enums;
using Mediaportal.TV.Server.TVLibrary.Interfaces;
using Mediaportal.TV.Server.TVLibrary.Interfaces.Implementations.Channels;
using Mediaportal.TV.Server.TVLibrary.Interfaces.Interfaces;

namespace Mediaportal.TV.Server.TVLibrary.Implementations.Analog
{
  /// <summary>
  /// An implementation of <see cref="IChannelScannerHelper"/> for analog channel scan logic.
  /// </summary>
  internal class ChannelScannerHelperAnalog : IChannelScannerHelper
  {
    /// <summary>
    /// Set or modify channel detail.
    /// </summary>
    /// <param name="channel">The channel.</param>
    public virtual void UpdateChannel(ref IChannel channel)
    {
      // Fill in missing names.
      if (string.IsNullOrWhiteSpace(channel.Name))
      {
        AnalogChannel analogChannel = channel as AnalogChannel;
        if (analogChannel != null)
        {
          if (analogChannel.MediaType == MediaTypeEnum.TV)
          {
            analogChannel.Name = string.Format("Analog TV {0}", analogChannel.ChannelNumber);
          }
          else if (analogChannel.MediaType == MediaTypeEnum.Radio)
          {
            analogChannel.Name = string.Format("FM {0}", ((float)analogChannel.Frequency / 1000000).ToString("F1"));
          }
        }
      }
      else
      {
        // Names pulled from German teletext often end with "text" (because we
        // are actually getting the teletext service name). Remove the suffix.
        Match m = Regex.Match(channel.Name, @"(.*?)\s*text$", RegexOptions.IgnoreCase);
        if (m.Success)
        {
          channel.Name = m.Groups[1].Captures[0].Value;
        }
      }
    }

    /// <summary>
    /// Get the correct media type for a channel.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="videoStreamCount">The number of video streams associated with the service.</param>
    /// <param name="audioStreamCount">The number of audio streams associated with the service.</param>
    public virtual MediaTypeEnum? GetMediaType(int serviceType, int videoStreamCount, int audioStreamCount)
    {
      if (videoStreamCount > 0)
      {
        return MediaTypeEnum.TV;
      }
      if (audioStreamCount > 0)
      {
        return MediaTypeEnum.Radio;
      }
      return null;
    }
  }
}