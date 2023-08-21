FROM golang AS build
WORKDIR /app
COPY . .
RUN go build -o api
CMD ["./api"]
