﻿using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi {
	
	public class api_start2 : APIBase {


		public override void OnResponseReceived( dynamic data ) {

			KCDatabase db = KCDatabase.Instance;


			//api_mst_ship
			foreach ( var elem in data.api_mst_ship ) {

				int id = (int)elem.api_id;
				if ( db.MasterShips[id] == null ) {
					var ship = new ShipDataMaster();
					ship.LoadFromResponse( APIName, elem );
					db.MasterShips.Add( ship );
				} else {
					db.MasterShips[id].LoadFromResponse( APIName, elem );
				}
			}


			//改装関連のデータ設定
			foreach ( var ship in db.MasterShips ) {
				int remodelID = ship.Value.RemodelAfterShipID;
				if ( remodelID != 0 ) {
					db.MasterShips[remodelID].RemodelBeforeShipID = ship.Key;
				}
			}


			//api_mst_slotitem_equiptype
			foreach ( var elem in data.api_mst_slotitem_equiptype ) {

				int id = (int)elem.api_id;
				if ( db.EquipmentTypes[id] == null ) {
					var eqt = new EquipmentType();
					eqt.LoadFromResponse( APIName, elem );
					db.EquipmentTypes.Add( eqt );
				} else {
					db.EquipmentTypes[id].LoadFromResponse( APIName, elem );
				}
			}


			//api_mst_stype
			foreach ( var elem in data.api_mst_stype ) {

				int id = (int)elem.api_id;
				if ( db.ShipTypes[id] == null ) {
					var spt = new ShipType();
					spt.LoadFromResponse( APIName, elem );
					db.ShipTypes.Add( spt );
				} else {
					db.ShipTypes[id].LoadFromResponse( APIName, elem );
				}
			}


			//api_mst_slotitem
			foreach ( var elem in data.api_mst_slotitem ) {

				int id = (int)elem.api_id;
				if ( db.MasterEquipments[id] == null ) {
					var eq = new EquipmentDataMaster();
					eq.LoadFromResponse( APIName, elem );
					db.MasterEquipments.Add( eq );
				} else {
					db.MasterEquipments[id].LoadFromResponse( APIName, elem );
				}
			}


			//api_mst_useitem
			foreach ( var elem in data.api_mst_useitem ) {

				int id = (int)elem.api_id;
				if ( db.MasterUseItems[id] == null ) {
					var item = new UseItemMaster();
					item.LoadFromResponse( APIName, elem );
					db.MasterUseItems.Add( item );
				} else {
					db.MasterUseItems[id].LoadFromResponse( APIName, elem );
				}
			}


			//api_mst_mapinfo
			foreach ( var elem in data.api_mst_mapinfo ) {

				int id = (int)elem.api_id;
				if ( db.MapInfo[id] == null ) {
					var item = new MapInfoData();
					item.LoadFromResponse( APIName, elem );
					db.MapInfo.Add( item );
				} else {
					db.MapInfo[id].LoadFromResponse( APIName, elem );
				}
			}


			//api_mst_shipupgrade
			foreach ( var elem in data.api_mst_shipupgrade ) {

				if ( elem.api_drawing_count > 0 ) {
					int id = db.MasterShips[(int)elem.api_id].RemodelBeforeShipID;
					if ( id != 0 ) {
						db.MasterShips[id].NeedBlueprint = (int)elem.api_drawing_count;
					}
				}
			}


			base.OnResponseReceived( (object)data );
		}

		public override string APIName {
			get { return "api_start2"; }
		}
	}

}
