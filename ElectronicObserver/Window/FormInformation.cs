﻿using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window {

	public partial class FormInformation : DockContent {


		public FormInformation( FormMain parent ) {
			InitializeComponent();
		
		
		}


		private void FormInformation_Load( object sender, EventArgs e ) {

			APIObserver o = APIObserver.Instance;

			APIReceivedEventHandler rec = ( string apiname, dynamic data ) => Invoke( new APIReceivedEventHandler( Updated ), apiname, data );

			o.APIList["api_port/port"].ResponseReceived += rec;
			o.APIList["api_req_member/get_practice_enemyinfo"].ResponseReceived += rec;
			o.APIList["api_get_member/picture_book"].ResponseReceived += rec;
			o.APIList["api_req_kousyou/createitem"].ResponseReceived += rec;
			o.APIList["api_get_member/mapinfo"].ResponseReceived += rec;
		}


		void Updated( string apiname, dynamic data ) {

			//fixme: あとでメソッドを分離させる

			switch ( apiname ) {

				case "api_port/port":
					TextInformation.Text = "";		//とりあえずクリア
					break;

				case "api_req_member/get_practice_enemyinfo": {
						//int exp = 0;

						//undone: 現段階では面倒過ぎるので後日実装

						StringBuilder sb = new StringBuilder();
						sb.AppendLine( "[演習情報]" );
						sb.AppendLine( "敵艦隊名 : " + data.api_deckname );

						TextInformation.Text = sb.ToString();

					} break;


				case "api_get_member/picture_book": {
						StringBuilder sb = new StringBuilder();
						sb.AppendLine( "[中破絵未回収]" );

						foreach ( dynamic elem in data.api_list ) {

							if ( !elem.IsDefined( "api_yomi" ) )
								break;		//ないほうは装備

							dynamic[] state = elem.api_state;
							for ( int i = 0; i < state.Length; i++ ) {
								if ( (int)state[i][1] == 0 ) {
									sb.AppendLine( KCDatabase.Instance.MasterShips[(int)elem.api_table_id[i]].Name );
								}
							}
						}

						TextInformation.Text = sb.ToString();
					} break;


				case "api_req_kousyou/createitem": {
						if ( (int)data.api_create_flag == 0 ) {

							StringBuilder sb = new StringBuilder();
							sb.AppendLine( "[開発失敗]" );
							sb.AppendLine( data.api_fdata );

							EquipmentDataMaster eqm = KCDatabase.Instance.MasterEquipments[int.Parse( ( (string)data.api_fdata ).Split( ",".ToCharArray() )[1] )];
							if ( eqm != null )
								sb.AppendLine( eqm.Name );


							TextInformation.Text = sb.ToString();
						}

					} break;


				case "api_get_member/mapinfo": {

					StringBuilder sb = new StringBuilder();
					sb.AppendLine( "[海域ゲージ]" );

					foreach ( dynamic elem in data ) {

						int mapID = (int)elem.api_id;
						MapInfoData map = KCDatabase.Instance.MapInfo[mapID];

						if ( map != null ) {
							if ( map.RequiredDefeatedCount != -1 && elem.api_defeat_count() ) {

								sb.AppendFormat( "{0}-{1} : 撃破 {2}/{3} 回", map.MapAreaID, map.MapInfoID, (int)elem.api_defeat_count, map.RequiredDefeatedCount );
								sb.AppendLine();

							} else if ( elem.api_eventmap() ) {

								sb.AppendFormat( "{0}-{1} : HP {2}/{3}", map.MapAreaID, map.MapInfoID, (int)elem.api_eventmap.api_now_maphp, (int)elem.api_eventmap.api_max_maphp );
								sb.AppendLine();

							}
						}
					}

					TextInformation.Text = sb.ToString();

					} break;
			}

		}



		protected override string GetPersistString() {
			return "Information";
		}
	
	}

}
