﻿using Newtonsoft.Json;
using System;
using DiscordRPC.Helper;

namespace DiscordRPC
{
	/// <summary>
	/// The Rich Presence structure that will be sent and received by Discord. Use this class to build your presence and update it appropriately.
	/// </summary>
	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	[Serializable]
	public class RichPresence
	{
		/// <summary>
		/// The user's current <see cref="Party"/> status. For example, "Playing Solo" or "With Friends".
		/// <para>Max 128 bytes</para>
		/// </summary>
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get { return _state; } set { _state = value.ClearEmpty(); } }
		private string _state;

		/// <summary>
		/// What the user is currently doing. For example, "Competitive - Total Mayhem".
		/// <para>Max 128 bytes</para>
		/// </summary>
		[JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
		public string Details { get { return _details; } set { _details = value.ClearEmpty(); } }
		private string _details;
		
		/// <summary>
		/// The time elapsed / remaining time data.
		/// </summary>
		[JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
		public Timestamps Timestamps { get; set; }

		/// <summary>
		/// The names of the images to use and the tooltips to give those images.
		/// </summary>
		[JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
		public Assets Assets { get; set; }
		
		/// <summary>
		/// The party the player is currently in. The <see cref="Party.ID"/> must be set for this to be included in the RichPresence update.
		/// </summary>
		[JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
		public Party Party { get; set; }
		
		/// <summary>
		/// The secrets used for Join / Spectate. Secrets are obfuscated data of your choosing. They could be match ids, player ids, lobby ids, etc. Make this object null if you do not wish too / unable too implement the Join / Request feature.
		/// <para>To keep security on the up and up, Discord requires that you properly hash/encode/encrypt/put-a-padlock-on-and-swallow-the-key-but-wait-then-how-would-you-open-it your secrets.</para>
		/// <para>Visit the <see href="https://discordapp.com/developers/docs/rich-presence/how-to#secrets">Rich Presence How-To</see> for more information.</para>
		/// </summary>
		[JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
		public Secrets Secrets { get; set; }
		
		/// <summary>
		/// Marks the <see cref="Secrets.MatchSecret"/> as a game session with a specific beginning and end. It was going to be used as a form of notification, but was replaced with the join feature. It may potentially have use in the future, but it currently has no use.
		/// <para>
		/// "TLDR it marks the matchSecret field as an instance, that is to say a context in game that’s not like a lobby state/not in game state. It was gonna he used for notify me, but we scrapped that for ask to join. We may put it to another use in the future. For now, don’t worry about it" - Mason (Discord API Server 14 / 03 / 2018)
		///	</para>
		/// </summary>
		[JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
		[Obsolete("This was going to be used, but was replaced by JoinSecret instead")]
		private bool Instance { get; set; }

		
		/// <summary>
		/// Overrides all properties on this rich presence with the modifications if they are not null on the modification. This will perform a clone on each field.
		/// </summary>
		/// <param name="modification">The modifications to apply. Leave fields as null to ignore them.</param>
		public void Apply(RichPresence modification)
		{
			State = modification.State != null ? modification.State.Clone() as string : State;
			Details = modification.Details != null ? modification.Details.Clone() as string : Details;

			Secrets = !modification.HasSecrets() ? Secrets : new Secrets()
			{
				//MatchSecret = modification.Secrets.MatchSecret?.Clone() as string,
				JoinSecret = modification.Secrets.JoinSecret != null ? modification.Secrets.JoinSecret.Clone() as string : Secrets.JoinSecret,
				SpectateSecret = modification.Secrets.SpectateSecret != null ? modification.Secrets.SpectateSecret.Clone() as string : Secrets.SpectateSecret
			};

			Timestamps = !modification.HasTimestamps() ? Timestamps : new Timestamps()
			{
				Start = modification.Timestamps.Start,
				End = modification.Timestamps.End
			};

			Assets = !modification.HasAssets() ? Assets : new Assets()
			{
				LargeImageKey = modification.Assets.LargeImageKey != null ? modification.Assets.LargeImageKey.Clone() as string : Assets.LargeImageKey,
				LargeImageText = modification.Assets.LargeImageText != null ? modification.Assets.LargeImageText.Clone() as string : Assets.LargeImageText,
				SmallImageKey = modification.Assets.SmallImageKey != null ? modification.Assets.SmallImageKey.Clone() as string : Assets.SmallImageKey,
				SmallImageText = modification.Assets.SmallImageText != null ? modification.Assets.SmallImageText.Clone() as string : Assets.SmallImageText
			};

			Party = !modification.HasParty() ? Party : new Party()
			{
				ID = modification.Party.ID as string,
				Size = modification.Party.Size,
				Max = modification.Party.Max
			};
		}

		/// <summary>
		/// Clones the presence into a new instance. Used for thread safe writing and reading. This function will ignore properties if they are in a invalid state.
		/// </summary>
		/// <returns></returns>
		public RichPresence Clone()
		{
			//Create a new presence and copy over all our settings that are not null
			RichPresence presence = new RichPresence();
			presence.Apply(this);

			//Return the clone
			return presence;
		}

		/// <summary>
		/// Does the Rich Presence have valid timestamps?
		/// </summary>
		/// <returns></returns>
		public bool HasTimestamps()
		{
			return this.Timestamps != null && (Timestamps.Start != null || Timestamps.End != null);
		}

		/// <summary>
		/// Does the Rich Presence have valid assets?
		/// </summary>
		/// <returns></returns>
		public bool HasAssets()
		{
			return this.Assets != null;
		}

		/// <summary>
		/// Does the Rich Presence have a valid party?
		/// </summary>
		/// <returns></returns>
		public bool HasParty()
		{
			return this.Party != null && this.Party.ID != null;
		}

		/// <summary>
		/// Does the Rich Presence have valid secrets?
		/// </summary>
		/// <returns></returns>
		public bool HasSecrets()
		{
			return Secrets != null && (Secrets.JoinSecret != null || Secrets.SpectateSecret != null);
		}

		/// <summary>
		/// Operator that converts a presence into a boolean for null checks.
		/// </summary>
		/// <param name="presesnce"></param>
		public static implicit operator bool(RichPresence presesnce)
		{
			return presesnce != null;
		}
	}
	
	/// <summary>
	/// The secrets used for Join / Spectate. Secrets are obfuscated data of your choosing. They could be match ids, player ids, lobby ids, etc.
	/// <para>To keep security on the up and up, Discord requires that you properly hash/encode/encrypt/put-a-padlock-on-and-swallow-the-key-but-wait-then-how-would-you-open-it your secrets.</para>
	/// <para>You should send discord data that someone else's game client would need to join or spectate their friend. If you can't or don't want to support those actions, you don't need to send secrets.</para>
	/// <para>Visit the <see href="https://discordapp.com/developers/docs/rich-presence/how-to#secrets">Rich Presence How-To</see> for more information.</para>
	/// </summary>
	[Serializable]
	public class Secrets
	{
		/// <summary>
		/// The unique match code to distinguish different games/lobbies. Use <see cref="Secret.CreateSecret()"/> to get an appropriately sized secret. 
		/// <para>This cannot be null and must be supplied for the  Join / Spectate feature to work.</para>
		/// <para>Max Length of 128 Bytes</para>
		/// </summary>
		[Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.")]
		[JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
		public string MatchSecret { get { return _matchSecret; } set { _matchSecret = value.ClearEmpty(); } }
		private string _matchSecret;


		/// <summary>
		/// The secret data that will tell the client how to connect to the game to play. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
		/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
		/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
		/// </para>
		/// <para>Max Length of 128 Bytes</para>
		/// </summary>
		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		public string JoinSecret { get { return _joinSecret; } set { _joinSecret = value.ClearEmpty(); } }
		private string _joinSecret;

		/// <summary>
		/// The secret data that will tell the client how to connect to the game to spectate. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
		/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
		/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
		/// </para>
		/// <para>Max Length of 128 Bytes</para>
		/// </summary>
		[JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
		public string SpectateSecret { get { return _spectateSecret; } set { _spectateSecret = value.ClearEmpty(); } }
		private string _spectateSecret;
	}

	[Serializable]
	public class Assets
	{
		/// <summary>
		/// Name of the uploaded image for the large profile artwork.
		/// <para>Max 32 Bytes.</para>
		/// </summary>
		[JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageKey { get { return _largeimagekey; } set { _largeimagekey = value.ClearEmpty(); } }		
		private string _largeimagekey;

		/// <summary>
		/// The tooltip for the large square image. For example, "Summoners Rift" or "Horizon Lunar Colony".
		/// <para>Max 128 Bytes.</para>
		/// </summary>
		[JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageText { get { return _largeimagetext; } set { _largeimagetext = value.ClearEmpty(); } }
		private string _largeimagetext;


		/// <summary>
		/// Name of the uploaded image for the small profile artwork.
		/// <para>Max 32 Bytes.</para>
		/// </summary>
		[JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageKey { get { return _smallimagekey; } set { _smallimagekey = value.ClearEmpty(); } }
		private string _smallimagekey;

		/// <summary>
		/// The tooltip for the small circle image. For example, "LvL 6" or "Ultimate 85%".
		/// <para>Max 128 Bytes.</para>
		/// </summary>
		[JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageText { get { return _smallimagetext; } set { _smallimagetext = value.ClearEmpty(); } }
		private string _smallimagetext;
	}

	/// <summary>
	/// Structure representing the start and endtimes of a match.
	/// </summary>
	[Serializable]
	public class Timestamps
	{
		/// <summary>
		/// The time that match started. Included this will show the time as "elapsed".
		/// </summary>
		[JsonIgnore]
		public DateTime? Start { get; set; }

		/// <summary>
		/// The time the match will end. Including this will show the time as "remaining".
		/// </summary>
		[JsonIgnore]
		public DateTime? End { get; set; }

		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		private long? epochStart { get { return Start.HasValue ? GetEpoch(Start.Value) : (long?)null; } }

		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		private long? epochEnd { get { return End.HasValue ? GetEpoch(End.Value) : (long?)null; } }

		/// <summary>
		/// Gets the Unix Epoch time equivilent of the DateTime.
		/// </summary>
		/// <param name="time">The time to convert to Unix Epoch</param>
		/// <returns>The Unix Epoch of the passed DateTime.</returns>
		public static long GetEpoch(DateTime time)
		{
			DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			return (long)(time - epochStart).TotalSeconds;
		}
	}

	/// <summary>
	/// Structure representing the part the player is in.
	/// </summary>
	[Serializable]
	public class Party
	{
		/// <summary>
		/// A unique ID for the player's current party / lobby / group. If this is not supplied, they player will not be in a party and the rest of the information will not be sent. 
		/// <para>Max 128 Bytes</para>
		/// </summary>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string ID { get { return _partyid; } set { _partyid = value.ClearEmpty(); } }
		private string _partyid;

		/// <summary>
		/// The current size of the players party / lobby / group.
		/// </summary>
		[JsonIgnore]
		public int Size { get; set; }

		/// <summary>
		/// The maxium size of the party / lobby / group. This is required to be larger than <see cref="Size"/>. If it is smaller than the current party size, it will automatically be set too <see cref="Size"/> when the presence is sent.
		/// </summary>
		[JsonIgnore]
		public int Max { get; set; }

		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		private int[] _size
		{
			get
			{
				//see issue https://github.com/discordapp/discord-rpc/issues/111
				int size = Math.Max(1, Size);
				return new int[] { size, Math.Max(size, Max) };
			}
		}
	}

	
	/// <summary>
	/// A rich presence that has been parsed from the pipe as a response.
	/// </summary>
	internal class RichPresenceResponse : RichPresence
	{
		/// <summary>
		/// ID of the client
		/// </summary>
		public string ClientID { get { return _clientid; } }

		/// <summary>
		/// Name of the bot
		/// </summary>
		public string Name { get { return _name; } }

		//Disabling warning that these are never assigned. These are assigned by the JSON converter.
#pragma warning disable 0649

		[JsonProperty("application_id")]
		private string _clientid;

		[JsonProperty("name")]
		private string _name;

#pragma warning restore 0649

	}
}
