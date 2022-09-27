local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")

local PlatinumBlock = {}

PlatinumBlock.name = "PlatinumStrawberry/PlatinumBlock"
PlatinumBlock.depth = -10000
PlatinumBlock.minimumSize = {16, 16}
PlatinumBlock.placements = {
    name = "default",
    data = {
        width = 16,
        height = 16
    }
}

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

local blockTexture = "SyrenyxPlatinumStrawberry/objects/platinumblock/block"
local iconTexture = "SyrenyxPlatinumStrawberry/objects/platinumblock/icon"

function PlatinumBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local ninePatch = drawableNinePatch.fromTexture(blockTexture, ninePatchOptions, x, y, width, height)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)
    local sprites = ninePatch:getDrawableSprite()

    iconSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, iconSprite)

    return sprites
end

return PlatinumBlock