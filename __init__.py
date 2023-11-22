from nonebot import require
import json
from nonebot.adapters import Bot, Event
from nonebot.typing import T_State
from nonebot import on_command
from nonebot import get_driver

from .config import Config
from .data_source import get_nuke_code_rougetrader

global_config = get_driver().config
config = Config.parse_obj(global_config)

scheduler = require("nonebot_plugin_apscheduler").scheduler
nukacode = on_command('核弹密码')


@nukacode.handle()
async def handle_nukacode(bot: Bot, event: Event, state: T_State):
    with open('/root/go-cqhttp_linux_386/BOS-bot-v3/bos_bot_v3/plugins/nukacode/codelog.json', 'r') as r:
        loaddict = json.load(r)
    code_stuff = loaddict['T_S']+' 至 '+loaddict['T_E']+'\nA点: ' + \
        loaddict['A']+'\nB点: '+loaddict['B']+'\nC点: '+loaddict['C']
    await nukacode.send(code_stuff)


@scheduler.scheduled_job("cron", hour="*/2", id="xxx", args=[1], kwargs={"arg2": 2})
async def run_every_2_hour(arg1, arg2):
    get_nuke_code_rougetrader()
    pass
