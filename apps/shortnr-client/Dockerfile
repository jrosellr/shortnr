FROM node:20.10.0 as base

WORKDIR /app
COPY package.json .
COPY package-lock.json .

FROM base as build
RUN npm ci

FROM build as release
COPY . .
EXPOSE 4200

CMD ["npm", "run", "start:container"]

