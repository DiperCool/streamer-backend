#!/usr/bin/env python3
import pika, sys, json, os
from datetime import datetime, timezone

try:
    uri = os.environ["RABBITMQ_URI"]
    params = pika.URLParameters(uri)
    conn = pika.BlockingConnection(params)
    ch = conn.channel()

    Id, Action, StreamName, VodId, Exchange, RoutingKey = sys.argv[1:7]
    
    # Current UTC timestamp in ISO 8601 format
    Timestamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

    payload = json.dumps({
        "Id": Id,
        "Action": Action,
        "StreamName": StreamName or None,
        "Timestamp": Timestamp,
        "VodId": VodId or None
    })

    ch.basic_publish(exchange=Exchange, routing_key=RoutingKey, body=payload)
    print(f"✅ Sent message to exchange '{Exchange}' with routing key '{RoutingKey}'")
    conn.close()
except Exception as e:
    print(f"❌ Error: {e}")
    sys.exit(1)
