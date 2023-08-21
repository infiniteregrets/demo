import discord
import aiohttp
from discord.ext import commands
import datetime
from dotenv import load_dotenv
import os
import logging

logging.basicConfig(level=logging.DEBUG)

load_dotenv()

bot = commands.Bot(
    command_prefix=commands.when_mentioned_or("!"),
    intents=discord.Intents.default(),
    help_command=None,
)


@bot.event
async def on_ready():
    print("Bot is ready!")


@bot.command()
async def library(ctx):
    logging.debug("Library command invoked")
    async with ctx.typing():
        async with aiohttp.ClientSession() as session:
            async with session.get("http://data-service:3000/library-stats") as r:
                embed = discord.Embed(
                    title="Library Stats",
                    description=f"`Live Stats! More live than the actual website`\n\n",
                    color=0x00FF00,
                    timestamp=datetime.datetime.utcnow(),
                )
                data = await r.json()
                for i, j in data["description"].items():
                    embed.add_field(name=i, value=j)

                await notify_logger()
                return await ctx.send(embed=embed)


async def notify_logger():
    async with aiohttp.ClientSession() as session:
        urls = [
            "http://logger-service:8080/api/v1",
            "http://logger-service:8080/api/v2",
            "http://logger-service:8080/api/v1",
            "http://logger-service:8080/api/v2",
        ]
        headers = [{"service-log": "true"}, {"service-log": "true"}, {"service-log": "true"}, {"service-log": "false"}]

        for url, header in zip(urls, headers):
            async with session.get(url, headers=header) as response:
                await response.text()
                if response.status == 200:
                    logging.debug(
                        f"Successfully sent request to {url} with headers {header}"
                    )
                else:
                    logging.debug(
                        f"Failed to send request to {url} with headers {header}. Status code: {response.status}"
                    )                


bot.run(os.getenv("TOKEN"))
