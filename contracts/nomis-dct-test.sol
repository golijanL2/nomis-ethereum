// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import "@openzeppelin/contracts@4.7.3/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts@4.7.3/token/ERC721/extensions/ERC721Burnable.sol";
import "@openzeppelin/contracts@4.7.3/access/Ownable.sol";

/// @custom:security-contact info@nomis.cc
contract NomisScore is ERC721, ERC721Burnable, Ownable {

    constructor() ERC721("NomisScore", "NMSS") {}

    struct Score {
        uint16 value;
        uint256 updated;
    }

    Score[] public scores;

    mapping (address => uint256) scoresMapping;

    function _baseURI() internal pure override returns (string memory) {
        return "https://tokens.nomis.cc/goerli/";
    }

    function setScore(uint16 score) public {

        address to = msg.sender;
        uint256 idx = scoresMapping[to];
        if(idx > 0)
        {
            idx--;
            scores[idx].value = score;
            scores[idx].updated = block.timestamp;
        }
        else
        {
            scores.push(Score(score, block.timestamp));
            uint256 tokenId = scores.length;
            _safeMint(to, tokenId);
            scoresMapping[to] = tokenId;
        }
    }

    function getScore(address addr) external view returns(Score memory)
    {
        uint256 idx = scoresMapping[addr];
        require(idx > 0, "No score for this address.");
        return scores[idx-1];
    }

    function _beforeTokenTransfer(address from, address to, uint256 tokenId)
        internal
        override (ERC721)
    {
        require(from == address(0) || to == address(0), "NonTransferrableERC721Token: score can't be transferred.");
        super._beforeTokenTransfer(from, to, tokenId);
    }

    function _afterTokenTransfer(address from, address to, uint256 tokenId
    ) internal override (ERC721)
    {
        if (to == address(0))
        {
            uint256 idx = scoresMapping[from] - 1;
            scores[idx].value = 0;
            scores[idx].updated = 0;
            delete scoresMapping[from];
        }

        super._afterTokenTransfer(from, to, tokenId);
    }

    function supportsInterface(bytes4 interfaceId)
        public
        view
        override(ERC721)
        returns (bool)
    {
        return super.supportsInterface(interfaceId);
    }
}
