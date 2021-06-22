import React, { useState, useEffect } from 'react';

import { Map, TileLayer, Marker, Popup } from 'react-leaflet';
import HeatmapLayer from 'react-leaflet-heatmap-layer';
import L from 'leaflet';
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

const Heatmap = () => {
    const [ips, setIps] = useState([]);
    const [updating, setUpdating] = useState(false);
    const [lowerLeftLat, setLowerLeftLat] = useState(51);
    const [lowerLeftLong, setLowerLeftLong] = useState(-5);
    const [upperRightLat, setUpperRightLat] = useState(53);
    const [upperRightLong, setUpperRightLong] = useState(0);

    /* Reset the ip list whenever the bounds are changed */
    useEffect(() => {
        setIps([]);
    }, [lowerLeftLat, lowerLeftLong, upperRightLat, upperRightLong]);

    /* When the IPs are updated, trigger this method. Only run if ips is empty */
    useEffect(() => {
        const fetchBoundedIps = async (page) => {
            const res = await fetch(`/api/ip?page=${page}&lowerLeftLat=${lowerLeftLat}&upperRightLat=${upperRightLat}&lowerLeftLong=${lowerLeftLong}&upperRightLong=${upperRightLong}`);
            const json = await res.json();
            return json;
        };

        const fetchData = async (page, ipCollection) => {
            let fetchedIps = await fetchBoundedIps(page);
            ipCollection = ipCollection.concat(fetchedIps);
            while (fetchedIps.length > 0) {
                page++;
                fetchedIps = await fetchBoundedIps(page);
                ipCollection = ipCollection.concat(fetchedIps);
            }
            setIps(ipCollection);
            setUpdating(false);
        }

        if (ips.length === 0 && !updating) {
            setUpdating(true);
            const tempIps = [];
            fetchData(1, tempIps);
        }
    }, [ips]);



    

    const DefaultIcon = L.icon({
        iconUrl: icon,
        shadowUrl: iconShadow
    });

    L.Marker.prototype.options.icon = DefaultIcon;

    return (
        <>
        {
            (ips === null)
            ? <div>Loading</div>
            : <Map
                center={[51.505, -0.09]}
                zoom={13}
                scrollWheelZoom={true}
                style={{ width: '80%', height: '70%' }}>

                <HeatmapLayer
                    fitBoundsOnLoad
                    fitBoundsOnUpdate
                    points={ips}
                    longitudeExtractor={ip => ip.longitude}
                    latitudeExtractor={ip => ip.latitude}
                    intensityExtractor={ip => ip.count}
                />
                <TileLayer
                    attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <Marker position={[51.505, -0.09]}>
                    <Popup>
                        A pretty CSS3 popup. <br /> Easily customizable.
                    </Popup>
                </Marker>
            </Map>
        }
        </>
    );
};

export default Heatmap;