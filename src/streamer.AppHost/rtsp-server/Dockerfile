FROM bluenviron/mediamtx:latest AS mediamtx-src

FROM alpine:3.20

RUN apk add --no-cache bash libc6-compat unzip openssl curl python3 py3-pip

COPY --from=mediamtx-src /mediamtx /usr/local/bin/mediamtx
COPY send_webhook.py /usr/local/bin/send_webhook.py

RUN python3 -m venv /opt/venv && \
    /opt/venv/bin/pip install --upgrade pip && \
    /opt/venv/bin/pip install pika

ENV PATH="/opt/venv/bin:$PATH"

ENTRYPOINT ["/usr/local/bin/mediamtx"]

CMD ["/mediamtx.yml"]
